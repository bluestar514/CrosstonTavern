﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInitializer
{
    static int INF = 100000000;
    static int NEG_INF = -100000000;

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
                        new EffectKnowledge(new WorldFactResource("#feature#", "#common_fish#", "#c_fish#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.35f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#r_fish#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "#rare_fish#", "#r_fish#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.15f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"algee" }, 1, 5)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("feature", ActionRole.recipient),
                new BindingPortString("c_fish", "#common_fish#"),
                new BindingPortString("r_fish", "#rare_fish#")
            }
        ) },
        {"talk", new GenericAction("talk", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -10, 10), true),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, 0, 2),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, 0, 2)
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -10, 10), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -2, 0),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -2, 0)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
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
            }

        ) },
        {"give_#item#", new GenericAction("give_#item#", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.loved, ChanceModifierItemOpinion.OpinionLevel.max),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, 2, 5),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.loved, "#item#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.liked, ChanceModifierItemOpinion.OpinionLevel.liked),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, 1, 3),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.liked, "#item#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.min, ChanceModifierItemOpinion.OpinionLevel.neutral),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, -3, -1),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.disliked, "#item#"))
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortInventoryItem("item", "a")
            }
        ) },
        {"buy_#item#", new GenericAction("buy_#item#", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateInventoryBound("#a#", "currency", "#item.cost#", INF.ToString()))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifier(),
                    new List<Effect>() {
                        new EffectInventoryStatic("#b#", "#item#", -1),
                        new EffectInventoryStatic("#a#", "#item#", 1),
                        new EffectInventoryBound("#a#", "currency", "-#item.cost#")
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortStockItem("item", "b")
            }
        ) },
        {"ask_out_to_#location#", new GenericAction("ask_out_to_#location#", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateAvailableNow("#a#", WorldTime.Time(1,30)))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#","#a#", Relationship.RelationType.friendly, 0, 5), true),
                    new List<Effect>() {
                        new EffectObligationNow("#a#","Date_At_#location#_With_#b#", WorldTime.Time(1, 30), true,
                                                    new GoalModule(
                                                            new List<GM_Precondition>(){
                                                                new GM_Precondition_Now(WorldTime.Time(1, 30))
                                                            },
                                                            new List<Goal>() {
                                                                new Goal(new StatePosition("#a#", "#location#"), 10),
                                                                new Goal(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 10, 20), 5),
                                                                new Goal(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 5, 20), 3)
                                                            },
                                                            "Date_At_#location#_With_#b#"
                                              )
                        ),
                        new EffectObligationNow("#b#","Date_At_#location#_With_#a#", WorldTime.Time(1, 30), true,
                                                    new GoalModule(
                                                            new List<GM_Precondition>(){
                                                                new GM_Precondition_Now(WorldTime.Time(1, 30))
                                                            },
                                                            new List<Goal>() {
                                                                new Goal(new StatePosition("#b#", "#location#"), 10),
                                                                new Goal(new StateSocial("#a#", "#b#", Relationship.RelationType.friendly, 10, 20), 5),
                                                                new Goal(new StateSocial("#a#", "#b#", Relationship.RelationType.romantic, 5, 20), 3)
                                                            },
                                                            "Date_At_#location#_With_#a#"
                                              )
                        ),
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 3),
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, 3)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortEntity("location", ActionRole.location_any)
            }
        ) },
        {"insult", new GenericAction("insult", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -3, -1),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("angry_from_being_insulted", EntityStatusEffectType.angry, 12, 3, new List<string>(){"#a#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), true),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -1, 4)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
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
                        new EffectStatusEffect("#b#", new EntityStatusEffect("pleased_from_a_compliment", EntityStatusEffectType.happy, 12, 3, new List<string>(){"#a#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), true),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.romantic, 1),
                        new EffectStatusEffect("#b#", new EntityStatusEffect("pleased_from_a_compliment", EntityStatusEffectType.happy, 12, 3, new List<string>(){"#a#"}))
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 3, 20), false),
                    new List<Effect>() {
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, -1)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
        ) },
        {"start_dating", new GenericAction("start_dating", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateRelation("#a#", "#b#", Relationship.RelationshipTag.dating), false)
            }),
            new List<Outcome>() {
                new Outcome( //success:
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 3, 20), true),
                    new List<Effect>() {
                        new EffectRelationship("#a#", "#b#", Relationship.RelationshipTag.dating, true, true),
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 2),
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.romantic, 2),
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, 2),
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.romantic, 2),
                    }
                ),
                new Outcome( //failure:
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 3, 20), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -4, -1),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, -4, -2),
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.romantic, -2, 0),
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -2, 0)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
        ) },
        {"ask_for_#item#", new GenericAction("ask_for_#item#", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(//success:
                    new ChanceModifierCombination(new List<ChanceModifier>() {
                        new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 0, 5), true),
                        new ChanceModifierBoolState(new StateInventoryStatic("#b#", "#item#", 1, INF), true)
                    }),
                    new List<Effect>() {
                        new EffectInventoryStatic("#b#", "#item#", -1),
                        new EffectInventoryStatic("#a#", "#item#", 1),
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 2)
                    }
                ),
                new Outcome( //doesn't have item:
                    new ChanceModifierCombination(new List<ChanceModifier>() {
                        new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 0, 5), true),
                        new ChanceModifierBoolState(new StateInventoryStatic("#b#", "#item#", 1, INF), false)
                    }),
                    new List<Effect>() {
                        new EffectGoal("#b#", new GoalModule(
                            new List<GM_Precondition>() {},
                            new List<Goal>() {
                                new Goal(new StateInventoryStatic("#a#", "#item#", 1, INF), 3),
                            }
                        )),
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 2)
                    }
                ),
                new Outcome( //doesn't want to give item
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 0, 5), false),
                    new List<Effect>() {
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, -1),
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, -1)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortInventoryItem("item", "_any_")
            }
         ) }
    };
}
