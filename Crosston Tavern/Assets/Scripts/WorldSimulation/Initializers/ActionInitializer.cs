using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionInitializer
{
    static int INF = 100000000;
    static int NEG_INF = -100000000;

    static int stepsInDay = 28;

    static int smallIncrease = 1;
    static int mediumIncrease = 5;
    static int largeIncrease = 10;

    public static Dictionary<string, GenericAction> actions = new Dictionary<string, GenericAction>() {
        {"fish", new GenericAction("fish", 1,
            new Precondition( new List<Condition>() {
                new Condition_IsState(new StateInventoryStatic("#a#", "fishing_rod", 1, INF))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(.5f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#c_fish#"}, 1, 1, new VerbilizationEffectItemGather("caught")),
                        new EffectKnowledge(new WorldFactResource("#feature#", "common_fish", "#c_fish#")),
                        new EffectSkill("#a#", "fishing", 2)
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.3f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#r_fish#"}, 1, 1, new VerbilizationEffectItemGather("caught")),
                        new EffectKnowledge(new WorldFactResource("#feature#", "rare_fish", "#r_fish#")),
                        new EffectSkill("#a#", "fishing", 3)
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.19f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"algee" }, 1, 5, new VerbilizationEffectItemGather("caught")),
                        new EffectSkill("#a#", "fishing", 1)
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.01f),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "fishing_rod", -1, new VerbilizationEffectItemGather("broke")),
                        new EffectSkill("#a#", "fishing", -1),
                        new EffectStatusEffect("#a#", new EntityStatusEffect("mad_fishing_rod_broke", EntityStatusEffectType.angry, stepsInDay, 3, new List<string>{ "#a#"}))
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("feature", ActionRole.recipient),
                new BindingPortString("c_fish", "#common_fish#"),
                new BindingPortString("r_fish", "#rare_fish#")
            },
            new VerbilizationActionResourceGathering("go fishing", "went fishing")
        ) },
        {"forage", new GenericAction("forage", 1,
            new Precondition( new List<Condition>() {

            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(.5f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#c_forage#"}, 1, 1, new VerbilizationEffectItemGather("found")),
                        new EffectKnowledge(new WorldFactResource("#feature#", "common_forage", "#c_forage#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.35f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#r_forage#"}, 1, 1, new VerbilizationEffectItemGather("found")),
                        new EffectKnowledge(new WorldFactResource("#feature#", "rare_forage", "#r_forage#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.15f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"dandelion" }, 1, 5, new VerbilizationEffectItemGather("found"))
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("feature", ActionRole.recipient),
                new BindingPortString("c_forage", "#common_forage#"),
                new BindingPortString("r_forage", "#rare_forage#")
            },
            new VerbilizationActionResourceGathering("go foraging","went foraging")
        ) },
        {"talk", new GenericAction("talk", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly, -20, 20), true),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.Axis.friendly, smallIncrease, mediumIncrease),
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.friendly, smallIncrease, mediumIncrease,
                                            new VerbilizationEffectSocialThreshold("we had a good time", 0, true))
                    }
                ),
                new Outcome(
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly, -20, 20), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.Axis.friendly, -mediumIncrease, -smallIncrease),
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.friendly, -mediumIncrease, -smallIncrease,
                                            new VerbilizationEffectSocialThreshold("it was frustrating", 0, false))
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionSocial("talk to", "talked to")
        ) },
        {"move", new GenericAction("move", 0,
            new Precondition(new List<Condition>() {
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifier(),
                    new List<Effect>() {
                        new EffectMovement("#a#", "#"+Map.R_CONNECTEDLOCATION+"#")
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator)
            },
            new VerbilizationMovement()

        ) },
        {"give_#item#", new GenericAction("give_#item#", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateInventoryStatic("#a#", "#item#", 1, INF))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.loved, ChanceModifierItemOpinion.OpinionLevel.max),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.romantic, 10, 15,
                                                                        new VerbilizationEffect("#b# loved it")),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.loved, "#item#")),
                        new EffectStatusEffect("#b#",
                            new EntityStatusEffect("happy_from_loved_gift", EntityStatusEffectType.happy,
                                                    stepsInDay*2, 5, new List<string>(){"#a#"}))
                        ,
                        new EffectStatusEffect("#a#",
                            new EntityStatusEffect("happy_gift_was_loved", EntityStatusEffectType.happy,
                                                    stepsInDay, 2, new List<string>(){"#b#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.liked, ChanceModifierItemOpinion.OpinionLevel.liked),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.romantic, 0, 5,
                                                                        new VerbilizationEffect("#b# liked it")),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.liked, "#item#")),
                        new EffectStatusEffect("#b#",
                            new EntityStatusEffect("happy_from_liked_gift", EntityStatusEffectType.happy,
                                                    stepsInDay*2, 2, new List<string>(){"#a#"})),
                        new EffectStatusEffect("#a#",
                            new EntityStatusEffect("happy_gift_was_liked", EntityStatusEffectType.happy,
                                                    stepsInDay, 1, new List<string>(){"#b#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.neutral, ChanceModifierItemOpinion.OpinionLevel.neutral),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.friendly, -5, 5,
                                                            new VerbilizationEffect("#b# didn't seem impressed")),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.neutral, "#item#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.min, ChanceModifierItemOpinion.OpinionLevel.disliked),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.romantic, -30, -15,
                                                                new VerbilizationEffect("#b# didn't like it at all")),
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.friendly, -20, -10),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.disliked, "#item#")),
                        new EffectStatusEffect("#b#",
                            new EntityStatusEffect("sad_from_disliked_gift", EntityStatusEffectType.sad,
                                                    stepsInDay*2, 1, new List<string>(){"#a#"})),
                        new EffectStatusEffect("#a#",
                            new EntityStatusEffect("angry_gift_was_not_apprciated", EntityStatusEffectType.angry,
                                                    stepsInDay, 3, new List<string>(){"#b#"}))
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortInventoryItem("item", "a")
            },
            new VerbilizationActionItemGive("give", "gave", "#item#")
        ) },
        {"ask_#item#", new GenericAction("ask_#item#", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateInventoryStatic("#b#", "#item#", 1, INF))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierCombination( new List<ChanceModifier>(){
                        new ChanceModifierItemOpinion("#item#", "#b#",
                                                        ChanceModifierItemOpinion.OpinionLevel.min,
                                                        ChanceModifierItemOpinion.OpinionLevel.neutral),
                        new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly, 3, 100), true)
                    }, ChanceModifierCombination.Mode.additive),
                    new List<Effect>() {
                        new EffectInventoryStatic("#b#", "#item#", -1, new VerbilizationEffect("#b# gave it happily")),
                        new EffectInventoryStatic("#a#", "#item#", 1)
                    }
                ),
                new Outcome(
                    new ChanceModifier(),
                    new List<Effect>() {
                        new EffectSocialStatic("#a#", "#b#", Relationship.Axis.friendly, -2,
                                                    new VerbilizationEffect("#b# wouldn't give it to #a#")),
                        new EffectSocialStatic("#b#", "#a#", Relationship.Axis.friendly, -2)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortInventoryItem("item", "b")
            },
            new VerbilizationActionItemAskFor("ask for", "asked for", "#item#")
        ) },
        { "buy_fishing_rod", new GenericAction("buy_fishing_rod", 1,
            new Precondition(new List<Condition>() {
                new Condition_IsState(new StateInventoryStatic("#a#", "fishing_rod", 0, 0))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifier(),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "fishing_rod", 1)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionResourceGathering("buy a fishing rod", "bought a fishing rod")
        )},
        {"insult", new GenericAction("insult", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly, 
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.disliked][Relationship.Axis.friendly][1],
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.friend][Relationship.Axis.friendly][0]), 
                                            false),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.friendly, -largeIncrease, -mediumIncrease,
                                            new VerbilizationEffectSocialThreshold("#b# got real mad",  0, false)),
                        //new EffectSocialVariable("#a#", "#b#", Relationship.Axis.friendly, -mediumIncrease, -smallIncrease),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("angry_from_being_insulted", EntityStatusEffectType.angry, stepsInDay, 3, new List<string>(){"#a#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly,
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.disliked][Relationship.Axis.friendly][1],
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.friend][Relationship.Axis.friendly][0]),
                                            true),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.friendly, -smallIncrease, smallIncrease,
                                                new VerbilizationEffectSocialThreshold("#b# ignored it", 0, true)),
                        new EffectSocialVariable("#a#", "#b#", Relationship.Axis.friendly, -mediumIncrease, smallIncrease)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionSocial("insult", "insulted")
        ) },
        {"compliment", new GenericAction("compliment", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.romantic, 
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.crushing_on][Relationship.Axis.romantic][0],
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.head_over_heels][Relationship.Axis.romantic][0]), 
                                             true),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.Axis.romantic, mediumIncrease,
                                            new VerbilizationEffectSocialThreshold("#b# blushed bright red",  0, true)),
                        //new EffectSocialStatic("#a#", "#b#", Relationship.Axis.romantic, mediumIncrease/2),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("pleased_from_a_compliment", EntityStatusEffectType.happy, stepsInDay, 3, new List<string>(){"#a#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly, 
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.friend][Relationship.Axis.friendly][0], 
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.bestFriend][Relationship.Axis.friendly][0]), 
                                            true),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.Axis.romantic, smallIncrease,
                                        new VerbilizationEffectSocialThreshold("#b# seemed pleased", 0, true)),
                        //new EffectSocialStatic("#a#", "#b#", Relationship.Axis.friendly, mediumIncrease),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("pleased_from_a_compliment", EntityStatusEffectType.happy, stepsInDay, 3, new List<string>(){"#a#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly,
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.friend][Relationship.Axis.friendly][0],
                                                (int)Relationship.codifiedRelationRanges[Relationship.Tag.bestFriend][Relationship.Axis.friendly][0]), 
                                            false),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.Axis.friendly, -smallIncrease),
                        new EffectSocialStatic("#b#", "#a#", Relationship.Axis.romantic, -mediumIncrease,
                                            new VerbilizationEffectSocialThreshold("#b# didn't believe it",  0, false))
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionSocial("compliment", "complimented")
        ) },
        {"ask_out", new GenericAction("ask_out", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateRelation("#a#", "#b#", Relationship.Tag.dating), false),
                new Condition_IsState(new StateRelation("#a#", "#b#", Relationship.Tag.in_love_with), true)
            }),
            new List<Outcome>() {
                new Outcome( //success:
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.romantic, 60, 100), true),
                    new List<Effect>() {
                        new EffectRelationship("#a#", "#b#", Relationship.Tag.dating, true, true)
                    }
                ),
                new Outcome( //failure:
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.romantic, 60, 100), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.friendly, -largeIncrease, -mediumIncrease),
                        new EffectSocialVariable("#b#", "#a#", Relationship.Axis.romantic, -largeIncrease*2, -largeIncrease),
                        new EffectSocialVariable("#a#", "#b#", Relationship.Axis.romantic, -mediumIncrease, 0),
                        new EffectSocialVariable("#a#", "#b#", Relationship.Axis.friendly, -mediumIncrease, 0)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }, 
            
            // I asked out alicia. 
            // why did you ask out alicia?
            new VerbilizationActionSocial("ask out", "asked out")
        ) },
/*        {"ask_if_friends", new GenericAction("ask_if_friends", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome( //success:
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly, 40, 40), true),
                    new List<Effect>() {
                        new EffectRelationship("#b#", "#a#", Relationship.Tag.friend, false, true)
                    }
                ),
                new Outcome( //failure:
                    new ChanceModifierSocial(new StateSocial("#b#", "#a#", Relationship.Axis.friendly, 40, 40), false),
                    new List<Effect>() {
                        new EffectRelationship("#b#", "#a#", Relationship.Tag.friend, false, false)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }, 
            
            // I asked out alicia. 
            // why did you ask out alicia?
            new VerbilizationActionSocial("ask if they are friends", "asked if they were friends")
        ) },*/
        { "tend_crops" , new GenericAction("tend_crops", 1,
            new Precondition(new List<Condition>(){
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectSkill("#b#", "vitality", 1)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbalizationActionFeatureAt("tend", "tended")
        )},
        { "harvest_crops" , new GenericAction("harvest_crops", 1,
            new Precondition(new List<Condition>(){
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSkill(new StateSkill("#b#", "vitality", 3, 4), false),
                    new List<Effect>() {
                        new EffectSkill("#b#", "vitality", -3)
                    }
                ),
                new Outcome(
                    new ChanceModifierSkill(new StateSkill("#b#", "vitality", 3, 5), true),
                    new List<Effect>() {
                        new EffectSkill("#b#", "vitality", -4),
                        new EffectInventoryVariable("#a#", new List<string>(){"#crop#"}, 1, 3)
                    }
                ),
                new Outcome(
                    new ChanceModifierSkill(new StateSkill("#b#", "vitality", 4, 8), true),
                    new List<Effect>() {
                        new EffectSkill("#b#", "vitality", -5),
                        new EffectInventoryVariable("#a#", new List<string>(){"#crop#"}, 4, 6)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortString("crop", "#planted#")
            },
            new VerbalizationActionFeatureAt("harvest", "harvested")
        )},
        { "tend_animal" , new GenericAction("tend_animal", 1,
            new Precondition(new List<Condition>(){
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSkill(new StateSkill("#b#", "vitality", 0, 10), false),
                    new List<Effect>() {
                        new EffectSkill("#b#", "vitality", 1)
                    }
                ),
                new Outcome(
                    new ChanceModifierSkill(new StateSkill("#b#", "vitality", 0, 10), true),
                    new List<Effect>() {
                        new EffectSkill("#b#", "vitality", -10),
                        new EffectInventoryVariable("#a#", new List<string>(){"#animal_product#"}, 1, 1)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortString("animal_product", "#produce#")
            },
            new VerbalizationActionFeatureAt("tend", "tended")
        )},
        { "sell_fish_at_market" , new GenericAction("sell_fish_at_market", 1,
            new Precondition(new List<Condition>(){
                new Condition_IsState(new StateInventoryStatic("#a#", "trout", 10, INF), true)
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectSkill("#a#", "fishing", 10),
                        new EffectInventoryStatic("#a#", "trout", -10)

                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator)
            },
            new VerbalizationActionFeatureAt("sell trout", "sold trout")
        ) },
        { "sell_crop_at_market" , new GenericAction("sell_crop_at_market", 1,
            new Precondition(new List<Condition>(){
                new Condition_IsState(new StateInventoryStatic("#a#", "flour", 10, INF), true)
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectSkill("#a#", "farming", 10),
                        new EffectInventoryStatic("#a#", "flour", -10)

                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator)
            },
            new VerbalizationActionFeatureAt("sell flour", "sold flour")
        ) },
        {"eat_#item#", new GenericAction("eat_#item#", 1,
            new Precondition(new List<Condition>() {
                new Condition_IsYou("#b#"),
                new Condition_IsState(new StateInventoryStatic("#a#", "#item#", 1, INF)),
                new Condition_IsItemClass("#item#", ItemInitializer.ItemClass.food)
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#a#",
                                                        ChanceModifierItemOpinion.OpinionLevel.min,
                                                        ChanceModifierItemOpinion.OpinionLevel.neutral),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "#item#", -1,
                                new VerbilizationEffect("#a# ate it grudgingly"))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#a#",
                                                        ChanceModifierItemOpinion.OpinionLevel.liked,
                                                        ChanceModifierItemOpinion.OpinionLevel.max),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "#item#", -1,
                                new VerbilizationEffect("#a# ate it happily")),
                        new EffectStatusEffect("#a#",
                            new EntityStatusEffect("ate_something_delicious", EntityStatusEffectType.happy,
                                                            stepsInDay, 1, new List<string>(){ }))
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortInventoryItem("item", "a")
            },
            new VerbalizationActionItemConsume("eat", "ate", "#item#")
        ) },
    };


    public static Dictionary<string, GenericAction> GetAllActions()
    {
        Dictionary<string, GenericAction> allActions = new Dictionary<string, GenericAction>( actions);

        //GeneratePlantingActions().ToList().ForEach(x => allActions.Add(x.Key, x.Value));
        GenerateRecipes().ToList().ForEach(x => allActions.Add(x.Key, x.Value));
        //GenerateRelationStatusActions().ToList().ForEach(x => allActions.Add(x.Key, x.Value));

        return allActions;
    }

    public static Dictionary<string, GenericAction> GenerateRecipes()
    {
        Dictionary<string, GenericAction> cookingActions = new Dictionary<string, GenericAction>();

        foreach (FoodItem food in ItemInitializer.menu.Values) {
            List<Condition> preconditions = new List<Condition>(
                                from ingredient in food.ingredients
                                select new Condition_IsState(new StateInventoryStatic("#a#", ingredient, 1, INF)));
            preconditions.Add(new Condition_NotYou("#b#"));

            List<Effect> effects = new List<Effect>(from ingredient in food.ingredients
                                                    select new EffectInventoryStatic("#a#", ingredient, -1));
            effects.Add(new EffectInventoryStatic("#a#", food.name, 1));

            cookingActions.Add(food.verb.verbPresent + "_" + food.name, 
                new GenericAction(food.verb.verbPresent+"_"+food.name, 1,
                            new Precondition(preconditions),
                            new List<Outcome>() {
                                new Outcome(
                                    new ChanceModifierSimple(1),
                                    effects)
                            },
                            new List<BindingPort>() {
                                new BindingPortEntity("a", ActionRole.initiator),
                                new BindingPortEntity("b", ActionRole.recipient)
                            }, 
                            food.verb
                            )
                );
        }

        return cookingActions;
    }

    static Dictionary<string, GenericAction> GenerateRelationStatusActions()
    {
        Dictionary<string, GenericAction> relationActions = new Dictionary<string, GenericAction>();

        Dictionary<Relationship.Tag, Dictionary<Relationship.Axis, float[]>> codifiedRelationRanges = 
                                                                                            Relationship.codifiedRelationRanges;

        foreach (Relationship.Tag tag in codifiedRelationRanges.Keys) {
            Dictionary<Relationship.Axis, float[]> relationRanges = codifiedRelationRanges[tag];

            
            string name = "ask_if_" + tag;

            relationActions.Add(name,
                                new GenericAction(name, 1,
                                    new Precondition(
                                        new List<Condition>() {
                                            new Condition_NotYou("#b#")
                                        }
                                    ),
                                    new List<Outcome>() {
                                        new Outcome( //success:
                                            new ChanceModifierRelation(new StateRelation("#b#", "#a#", tag), true),
                                            new List<Effect>() {
                                                new EffectRelationship("#b#", "#a#", tag, false, true)
                                            }
                                        ),
                                        new Outcome( //failure:
                                            new ChanceModifierRelation(new StateRelation("#b#", "#a#", tag), false),
                                            new List<Effect>() {
                                                new EffectRelationship("#b#", "#a#", tag, false, false)
                                            }
                                        ),
                                    },
                                    new List<BindingPort>() {
                                        new BindingPortEntity("a", ActionRole.initiator),
                                        new BindingPortEntity("b", ActionRole.recipient)
                                    },
                                    new VerbilizationActionSocial("ask if they are " + tag, "asked if they were " + tag)
                                )); 
        }


        return relationActions;
    }

    public static HashSet<string> GetAllActionGeneratedItems()
    {
        HashSet<string> items = new HashSet<string>();

        foreach(GenericAction action in GetAllActions().Values) {
            foreach(Outcome outcome in action.potentialOutcomes) {
                foreach(Effect effect in outcome.effects) {
                    if(effect is EffectInventory) {
                        EffectInventory effectInventory = (EffectInventory)effect;
                        if (!effectInventory.itemId.Contains("#")) {
                            items.Add(effectInventory.itemId);
                        }
                    }
                }
            }
        }

        return items;
    }


    public static List<GenericAction> GetAllPeopleActions()
    {
        List<GenericAction> peopleActions = new List<GenericAction>() {
            actions["talk"],
            actions["give_#item#"],
            actions["insult"],
            actions["compliment"],
            actions["ask_out"],
            actions["ask_#item#"],
            actions["eat_#item#"]
        };
/*
        foreach(KeyValuePair<string, GenericAction> pair in GetAllActions()) {
            if (pair.Key.StartsWith( "ask_if_"))
                peopleActions.Add(pair.Value);
        }*/

        return peopleActions;
    }
}
