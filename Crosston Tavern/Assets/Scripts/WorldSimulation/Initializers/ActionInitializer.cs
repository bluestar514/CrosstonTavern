using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInitializer
{
    static int INF = 100000000;
    static int NEG_INF = -100000000;

    static int stepsInDay = 28; 

    public static Dictionary<string, GenericAction> actions = new Dictionary<string, GenericAction>() {
        {"fish", new GenericAction("fish", 1,
            new Precondition( new List<Condition>() {
                new Condition_IsState(new StateInventoryStatic("#a#", "fishing_rod", 1, INF))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(.5f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#c_fish#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "common_fish", "#c_fish#")),
                        new EffectSkill("#a#", "fishing", 2)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectItemGather("caught")
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.35f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#r_fish#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "rare_fish", "#r_fish#")),
                        new EffectSkill("#a#", "fishing", 3)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectItemGather("caught")
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.15f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"algee" }, 1, 5),
                        new EffectSkill("#a#", "fishing", 1)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectItemGather("caught")
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
                        new EffectInventoryVariable("#a#", new List<string>(){"#c_forage#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "common_forage", "#c_forage#"))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectItemGather("found")
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.35f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#r_forage#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "rare_forage", "#r_forage#"))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectItemGather("found")
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.15f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"dandelion" }, 1, 5)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectItemGather("found")
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
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -10, 10), true),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, 1, 3),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, 1, 3)
                    },
                    new List<VerbilizationEffect>() {
                        //I talked to bob and we had a good time
                        new VerbilizationEffectSocialThreshold("we had a good time", "it was frustrating", 0)
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -10, 10), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -3, -1),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -3, -1)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectSocialThreshold("we had a good time", "it was frustrating", 0)
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
                    },
                    new List<VerbilizationEffect>() {
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
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, 10, 15),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.loved, "#item#"))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffect("#b# loved it")
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.liked, ChanceModifierItemOpinion.OpinionLevel.liked),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, 0, 5),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.liked, "#item#"))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffect("#b# liked it")
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.min, ChanceModifierItemOpinion.OpinionLevel.neutral),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, -30, -15),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -20, -10),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.disliked, "#item#"))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffect("#b# didn't like it at all")
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
                        new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 100), true)
                    }, ChanceModifierCombination.Mode.additive),
                    new List<Effect>() {
                        new EffectInventoryStatic("#b#", "#item#", -1),
                        new EffectInventoryStatic("#a#", "#item#", 1)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffect("#b# gave it happily")
                    }
                ),
                new Outcome(
                    new ChanceModifier(),
                    new List<Effect>() {
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, -2),
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, -2)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffect("#b# wouldn't give it to #a#")
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
                    },
                    new List<VerbilizationEffect>() {
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionResourceGathering("buy a fishing rod", "bought a fishing rod")
        )},
        //{"buy_#item#", new GenericAction("buy_#item#", 1,
        //    new Precondition(new List<Condition>() {
        //        new Condition_NotYou("#b#"),
        //        new Condition_IsState(new StateInventoryBound("#a#", "currency", "#item.cost#", INF.ToString()))
        //    }),
        //    new List<Outcome>() {
        //        new Outcome(
        //            new ChanceModifier(),
        //            new List<Effect>() {
        //                new EffectInventoryStatic("#b#", "#item#", -1),
        //                new EffectInventoryStatic("#a#", "#item#", 1),
        //                new EffectInventoryBound("#a#", "currency", "-#item.cost#")
        //            }
        //        )
        //    },
        //    new List<BindingPort>() {
        //        new BindingPortEntity("a", ActionRole.initiator),
        //        new BindingPortEntity("b", ActionRole.recipient),
        //        new BindingPortStockItem("item", "b")
        //    }
        //) },
        //{"ask_out_to_#location#", new GenericAction("ask_out_to_#location#", 1,
        //    new Precondition(new List<Condition>() {
        //        new Condition_NotYou("#b#"),
        //        new Condition_IsState(new StateAvailableNow("#a#", WorldTime.Time(1,30)))
        //    }),
        //    new List<Outcome>() {
        //        new Outcome(
        //            new ChanceModifierRelation(new StateSocial("#b#","#a#", Relationship.RelationType.friendly, 0, 5), true),
        //            new List<Effect>() {
        //                new EffectObligationNow("#a#","Date_At_#location#_With_#b#", WorldTime.Time(1, 30), true,
        //                                            new GoalModule(
        //                                                    new List<GM_Precondition>(){
        //                                                        new GM_Precondition_Now(WorldTime.Time(1, 30))
        //                                                    },
        //                                                    new List<Goal>() {
        //                                                        new Goal(new StatePosition("#a#", "#location#"), 10),
        //                                                        new Goal(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 10, 20), 5),
        //                                                        new Goal(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 5, 20), 3)
        //                                                    },
        //                                                    "Date_At_#location#_With_#b#"
        //                                      )
        //                ),
        //                new EffectObligationNow("#b#","Date_At_#location#_With_#a#", WorldTime.Time(1, 30), true,
        //                                            new GoalModule(
        //                                                    new List<GM_Precondition>(){
        //                                                        new GM_Precondition_Now(WorldTime.Time(1, 30))
        //                                                    },
        //                                                    new List<Goal>() {
        //                                                        new Goal(new StatePosition("#b#", "#location#"), 10),
        //                                                        new Goal(new StateSocial("#a#", "#b#", Relationship.RelationType.friendly, 10, 20), 5),
        //                                                        new Goal(new StateSocial("#a#", "#b#", Relationship.RelationType.romantic, 5, 20), 3)
        //                                                    },
        //                                                    "Date_At_#location#_With_#a#"
        //                                      )
        //                ),
        //                new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 3),
        //                new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, 3)
        //            }
        //        )
        //    },
        //    new List<BindingPort>() {
        //        new BindingPortEntity("a", ActionRole.initiator),
        //        new BindingPortEntity("b", ActionRole.recipient),
        //        new BindingPortEntity("location", ActionRole.location_any)
        //    }
        //) },
        {"insult", new GenericAction("insult", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -3, -1),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("angry_from_being_insulted", EntityStatusEffectType.angry, stepsInDay, 3, new List<string>(){"#a#"}))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectSocialThreshold("#b# ignored it", "#b# got real mad",  0)
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), true),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -2, 1)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectSocialThreshold("#b# ignored it", "#b# got real mad",  0)
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
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 3, 10), true),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.romantic, 2),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("pleased_from_a_compliment", EntityStatusEffectType.happy, stepsInDay, 3, new List<string>(){"#a#"}))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectSocialThreshold("#b# blushed bright red", "#b# didn't believe it",  0)
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), true),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.romantic, 1),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("pleased_from_a_compliment", EntityStatusEffectType.happy, stepsInDay, 3, new List<string>(){"#a#"}))
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectSocialThreshold("#b# seemed pleased", "#b# didn't believe it",  0)
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), false),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, -3)
                    },
                    new List<VerbilizationEffect>() {
                        new VerbilizationEffectSocialThreshold("#b# seemed pleased", "#b# didn't believe it",  0)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionSocial("compliment", "complimented")
        ) },
        {"start_dating", new GenericAction("start_dating", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateRelation("#a#", "#b#", Relationship.RelationshipTag.dating), false),
                new Condition_IsState(new StateSocial("#a#", "#b#", Relationship.RelationType.romantic, 60, 1000), true)
            }),
            new List<Outcome>() {
                new Outcome( //success:
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 60, 100), true),
                    new List<Effect>() {
                        new EffectRelationship("#a#", "#b#", Relationship.RelationshipTag.dating, true, true)
                    },
                    new List<VerbilizationEffect>() {
                    }
                ),
                new Outcome( //failure:
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 60, 100), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -10, -5),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, -15, -5),
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.romantic, -5, 0),
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -5, 0)
                    },
                    new List<VerbilizationEffect>() {
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
        //{ "outing_shopping_at_#loc#_with_#b#", new GenericAction( "outing_shopping_at_#loc#_with_#b#", 1, 
        
        //    new Precondition(new List<Condition>() {
        //        new Condition_NotYou("#b#")
        //    }),
        //    new List<Outcome>() {
        //        new Outcome(
        //            new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -100, 0), true),
        //            new List<Effect>() {
        //                new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, 0, 10),
        //                new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, 0, 10)
        //            }
        //        ),
        //         new Outcome(
        //            new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -100, 0), false),
        //            new List<Effect>() {
        //                new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -10, -2),
        //                new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -10, -2)
        //            }
        //        )
        //    },
        //    new List<BindingPort>() {
        //        new BindingPortEntity("a", ActionRole.initiator),
        //        new BindingPortEntity("b", ActionRole.bystander),
        //        new BindingPortEntity("loc", ActionRole.recipient)
        //    }
        //)},
        { "bake_strawberry_cake", new GenericAction("bake_stawrberry_cake", 1, 
            new Precondition(new List<Condition>() {
                new Condition_IsState(new StateInventoryStatic("#a#", "strawberry", 1, INF)),
                new Condition_IsState(new StateInventoryStatic("#a#", "strawberry_cake_recipe", 1, INF)),
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "strawberry_cake", 1),
                        new EffectInventoryStatic("#a#", "strawberry", -1)
                    },
                    new List<VerbilizationEffect>() {
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }, // bob baked a strawberry cake

            new VerbilizationActionResourceGathering("bake a strawberry cake", "baked a strawberry cake") 
        )},
        { "fry_salmon", new GenericAction("fry_salmon", 1,
            new Precondition(new List<Condition>() {
                new Condition_IsState(new StateInventoryStatic("#a#", "salmon", 1, INF)),
                new Condition_IsState(new StateInventoryStatic("#a#", "fried_salmon_recipe", 1, INF)),
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "fried_salmon", 1),
                        new EffectInventoryStatic("#a#", "salmon", -1)
                    },
                    new List<VerbilizationEffect>() {
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionResourceGathering("make fried salmon", "made fried salmon")
        )},
        { "bake_blackberry_tart", new GenericAction("bake_blackberry_tart", 1,
            new Precondition(new List<Condition>() {
                new Condition_IsState(new StateInventoryStatic("#a#", "blackberry", 1, INF)),
                new Condition_IsState(new StateInventoryStatic("#a#", "blackberry_tart_recipe", 1, INF)),
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "blackberry_tart", 1),
                        new EffectInventoryStatic("#a#", "blackberry", -1)
                    },
                    new List<VerbilizationEffect>() {
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionResourceGathering("bake a blackberry tart", "baked a blackberry tart")
        )},
        { "stew_trout", new GenericAction("stew_trout", 1,
            new Precondition(new List<Condition>() {
                new Condition_IsState(new StateInventoryStatic("#a#", "trout", 1, INF)),
                new Condition_IsState(new StateInventoryStatic("#a#", "trout_stew_recipe", 1, INF)),
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "trout_stew", 1),
                        new EffectInventoryStatic("#a#", "trout", -1)
                    },
                    new List<VerbilizationEffect>() {
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationActionResourceGathering("make stewed trout", "made stewed trout")
        )},
        { "treat_sick_patient", new GenericAction("treat_sick_patient", 1,
            new Precondition(new List<Condition>(){
                new Condition_IsState(new StateInventoryStatic("#a#", "medicine", 1, INF))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectStatusModify("#b#", EntityStatusEffectType.sick, -2, -10),
                        new EffectInventoryStatic("#a#", "medicine", -1),
                        new EffectStatusEffect("#b#", 
                                new EntityStatusEffect("medically_treated", EntityStatusEffectType.special, 
                                                        stepsInDay*3, 1, new List<string>(){ "#a#" })
                            )
                    },
                    new List<VerbilizationEffect>() {
                        
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            },
            new VerbilizationAction("treat", "treated")
        )},
        { "wait_for_treatment", new GenericAction("wait_for_treatment", 1,
            new Precondition(new List<Condition>(){
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                    },
                    new List<VerbilizationEffect>(){ }
                )
            },
            new List<BindingPort>() {
            },
            new VerbilizationAction("wait", "waited")
        )},
        { "mix_medicine", new GenericAction("mix_medicine", 1,
            new Precondition(new List<Condition>(){
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(1),
                    new List<Effect>() {
                        new EffectInventoryStatic("#a#", "herb", -3),
                        new EffectInventoryStatic("#a#", "medicine", 1)
                    },
                    new List<VerbilizationEffect>(){ }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator)
            },
            new VerbilizationAction("make medicine", "made medicine")
        )},
        { "play_by_river", new GenericAction("play_by_river", 1,
            new Precondition(new List<Condition>(){
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(.6f),
                    new List<Effect>() {
                        
                    },
                    new List<VerbilizationEffect>(){ }
                ),
                new Outcome(
                    new ChanceModifierSimple(.4f),
                    new List<Effect>() {
                        new EffectStatusEffect("#a#", 
                                    new EntityStatusEffect("sick_from_wet", EntityStatusEffectType.sick, 
                                                                stepsInDay*7, 10, new List<string>(){"#a#"})),
                        //new Effect
                    },
                    new List<VerbilizationEffect>(){ }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator)
            },
            new VerbilizationAction("make medicine", "made medicine")
        )}
    };


    public static HashSet<string> GetAllActionGeneratedItems()
    {
        HashSet<string> items = new HashSet<string>();

        foreach(GenericAction action in actions.Values) {
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
}
