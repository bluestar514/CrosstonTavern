using System.Collections;
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
                        new EffectKnowledge(new WorldFactResource("#feature#", "common_fish", "#c_fish#")),
                        new EffectSkill("#a#", "fishing", 2)
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.35f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#r_fish#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "rare_fish", "#r_fish#")),
                        new EffectSkill("#a#", "fishing", 3)
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.15f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"algee" }, 1, 5),
                        new EffectSkill("#a#", "fishing", 1)
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
        {"forage", new GenericAction("forage", 1,
            new Precondition( new List<Condition>() {
                
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierSimple(.5f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#c_forage#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "common_forage", "#c_forage#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.35f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"#r_forage#"}, 1, 1),
                        new EffectKnowledge(new WorldFactResource("#feature#", "rare_forage", "#r_forage#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierSimple(.15f),
                    new List<Effect>() {
                        new EffectInventoryVariable("#a#", new List<string>(){"dandelion" }, 1, 5)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("feature", ActionRole.recipient),
                new BindingPortString("c_forage", "#common_forage#"),
                new BindingPortString("r_forage", "#rare_forage#")
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
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, 0, 3),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, 0, 3)
                    }
                ),
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -10, 10), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -3, 0),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -3, 0)
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
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, 10, 15),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.loved, "#item#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.liked, ChanceModifierItemOpinion.OpinionLevel.liked),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, 0, 5),
                        new EffectInventoryStatic("#b#", "#item#", 1),
                        new EffectInventoryStatic("#a#", "#item#", -1),
                        new EffectKnowledge(new WorldFactPreference("#b#", PreferenceLevel.liked, "#item#"))
                    }
                ),
                new Outcome(
                    new ChanceModifierItemOpinion("#item#", "#b#", ChanceModifierItemOpinion.OpinionLevel.min, ChanceModifierItemOpinion.OpinionLevel.neutral),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, -15, -10),
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
        {"ask_#item#", new GenericAction("ask_#item#", 1,
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#"),
                new Condition_IsState(new StateInventoryStatic("#b#", "#item#", 1, INF))
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifier(),
                    new List<Effect>() {
                        new EffectInventoryStatic("#b#", "#item#", -1),
                        new EffectInventoryStatic("#a#", "#item#", 1)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient),
                new BindingPortInventoryItem("item", "b")
            }
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
            }
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
                new Condition_IsState(new StateRelation("#a#", "#b#", Relationship.RelationshipTag.dating), false),
                new Condition_IsState(new StateSocial("#a#", "#b#", Relationship.RelationType.romantic, 20, 1000), true)
            }),
            new List<Outcome>() {
                new Outcome( //success:
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 20, 100), true),
                    new List<Effect>() {
                        new EffectRelationship("#a#", "#b#", Relationship.RelationshipTag.dating, true, true),
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 2),
                        new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.romantic, 2),
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, 2),
                        new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.romantic, 2),
                    }
                ),
                new Outcome( //failure:
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.romantic, 20, 100), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -15, -5),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.romantic, -15, -5),
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.romantic, -5, 0),
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -5, 0)
                    }
                ),
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
        ) },
        { "outing_shopping_at_#loc#_with_#b#", new GenericAction( "outing_shopping_at_#loc#_with_#b#", 1, 
        
            new Precondition(new List<Condition>() {
                new Condition_NotYou("#b#")
            }),
            new List<Outcome>() {
                new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -100, 0), true),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, 0, 10),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, 0, 10)
                    }
                ),
                 new Outcome(
                    new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, -100, 0), false),
                    new List<Effect>() {
                        new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -10, -2),
                        new EffectSocialVariable("#b#", "#a#", Relationship.RelationType.friendly, -10, -2)
                    }
                )
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.bystander),
                new BindingPortEntity("loc", ActionRole.recipient)
            }
        )},
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
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
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
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
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
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
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
                    })
            },
            new List<BindingPort>() {
                new BindingPortEntity("a", ActionRole.initiator),
                new BindingPortEntity("b", ActionRole.recipient)
            }
        )}
        
        //,
        //{"ask_for_#item#", new GenericAction("ask_for_#item#", 1,
        //    new Precondition(new List<Condition>() {
        //        new Condition_NotYou("#b#")
        //    }),
        //    new List<Outcome>() {
        //        new Outcome(//success:
        //            new ChanceModifierCombination(new List<ChanceModifier>() {
        //                new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 0, 5), true),
        //                new ChanceModifierBoolState(new StateInventoryStatic("#b#", "#item#", 1, INF), true)
        //            }),
        //            new List<Effect>() {
        //                new EffectInventoryStatic("#b#", "#item#", -1),
        //                new EffectInventoryStatic("#a#", "#item#", 1),
        //                new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 2)
        //            }
        //        ),
        //        new Outcome( //doesn't have item:
        //            new ChanceModifierCombination(new List<ChanceModifier>() {
        //                new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 0, 5), true),
        //                new ChanceModifierBoolState(new StateInventoryStatic("#b#", "#item#", 1, INF), false)
        //            }),
        //            new List<Effect>() {
        //                new EffectGoal("#b#", new GoalModule(
        //                    new List<GM_Precondition>() {},
        //                    new List<Goal>() {
        //                        new Goal(new StateInventoryStatic("#a#", "#item#", 1, INF), 3),
        //                    }
        //                )),
        //                new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 2)
        //            }
        //        ),
        //        new Outcome( //doesn't want to give item
        //            new ChanceModifierRelation(new StateSocial("#b#", "#a#", Relationship.RelationType.friendly, 0, 5), false),
        //            new List<Effect>() {
        //                new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, -1),
        //                new EffectSocialStatic("#b#", "#a#", Relationship.RelationType.friendly, -1)
        //            }
        //        )
        //    },
        //    new List<BindingPort>() {
        //        new BindingPortEntity("a", ActionRole.initiator),
        //        new BindingPortEntity("b", ActionRole.recipient),
        //        new BindingPortInventoryItem("item", "_any_")
        //    }
        // ) }
    };
}
