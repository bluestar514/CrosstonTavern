using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ActionBuilder_Tests
    {
        [Test]
        public void SimpleDeclareEffects()
        {
            //Assert.IsTrue(true, "true");


            EffectInventoryStatic getGoldfish = new EffectInventoryStatic("#a#", "goldfish", 1);
            EffectInventoryVariable getAlgee = new EffectInventoryVariable("#a#", new List<string>() { "algee" }, 1, 5);
            EffectSocialStatic makeFriends = new EffectSocialStatic("#a#", "#b#", Relationship.RelationType.friendly, 2);
            EffectSocialVariable tryToMakeFriends = new EffectSocialVariable("#a#", "#b#", Relationship.RelationType.friendly, -2, 2);
            EffectMovement move = new EffectMovement("#a#", "#connected_space#");

        }

        [Test]
        public void SimpleDeclareGenericAction() {


            GenericAction action = new GenericAction("fish", 1,
                new Precondition( new List<Condition>() {
                    new Condition_IsState(new StateInventory("#a#", "fishing_rod", 1, 100000000))
                }),
                new List<Outcome>() {
                    new Outcome(
                        new ChanceModifierSimple(.5f),
                        new List<Effect>() {
                            new EffectInventoryStatic("#a#", "goldfish", 1)
                        }
                    ),
                    new Outcome(
                        new ChanceModifierSimple(.5f),
                        new List<Effect>() {
                            new EffectInventoryVariable("#a#", new List<string>(){"algee" }, 1, 5)
                        }
                    )
                },
                new List<BindingPort>() {
                    new BindingPortEntity("a", ActionRole.initiator)
                }
            );
        }

        [Test]
        public void GenerateBoundActions_OneResult()
        {
            Person person = new Person("alicia");
            

            Map map = new Map(
                new List<Feature>() {
                    new Feature("fishing pool", "1", 2, 
                        new List<GenericAction>() {
                            ActionInitializer.actions["fish"]
                        },
                        new Dictionary<string, List<string>>() {

                        }
                    ),
                    person.feature
                },
                new List<Location>() {
                    new Location("1", new Dictionary<string, List<string>>() {})
                }
            );

            map.MovePerson(person, "1", false);

            Assert.AreEqual("1", person.location);
            Assert.AreEqual("1", person.feature.location);

            Registry registry = new Registry(new List<Person>() {
                person
            });
            WorldState worldState = new WorldState(map, registry, WorldTime.DayZeroEightAM);

            ActionBuilder ab = new ActionBuilder(worldState, person);
            List<ActionBuilder.ActionData> actionData = ab.GatherProvidedActionsForActorAt("1");
            Assert.AreEqual(1, actionData.Count);

            List<BoundAction> allActions = ab.GetAllActions("1");

            Assert.AreEqual(1, allActions.Count, "Action Count Miss Match"+ string.Join(",", allActions));

            Assert.AreEqual(1, allActions[0].Bindings.Count, string.Join(",", allActions[0].Bindings));

            Assert.AreEqual("<a:alicia>", allActions[0].Bindings.bindings[0].ToString());
        }

        [Test]
        public void GenerateBoundActions_MoreResults()
        {
            Person person = new Person("alicia");
            Person bob = new Person("bob");


            Map map = new Map(
                new List<Feature>() {
                    new Feature("fishing pool", "1", 2,
                        new List<GenericAction>() {
                            ActionInitializer.actions["fish"]
                        },
                        new Dictionary<string, List<string>>() {

                        }
                    ),
                    person.feature,
                    bob.feature
                },
                new List<Location>() {
                    new Location("1", new Dictionary<string, List<string>>() {})
                }
            );
            person.feature.providedActions.Add(ActionInitializer.actions["talk"]);
            bob.feature.providedActions.Add(ActionInitializer.actions["talk"]);

            map.MovePerson(person, "1", false);
            map.MovePerson(bob, "1", false);

            Registry registry = new Registry(new List<Person>() {
                person,
                bob
            });
            WorldState worldState = new WorldState(map, registry, WorldTime.DayZeroEightAM);

            ActionBuilder ab = new ActionBuilder(worldState, person);
            List<ActionBuilder.ActionData> actionData = ab.GatherProvidedActionsForActorAt("1");
            Assert.AreEqual(3, actionData.Count);

            List<BoundAction> allActions = ab.GetAllActions("1");

            Assert.AreEqual(3, allActions.Count, string.Join(",", allActions));
            Assert.AreEqual(2, allActions[1].Bindings.Count);

            Assert.AreEqual("<a:alicia>,<b:person_bob>", string.Join(",",allActions[2].Bindings.bindings));
        }

        [Test]
        public void GenerateBoundActions_InventoryItem_One()
        {
            Person person = new Person("alicia");
            Person bob = new Person("bob");


            Map map = new Map(
                new List<Feature>() {
                    //person.feature, //removed so there is only one feature in the room
                    bob.feature
                },
                new List<Location>() {
                    new Location("1", new Dictionary<string, List<string>>() {})
                }
            );
            person.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);
            bob.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);

            person.inventory.ChangeInventoryContents(3, "taco");

            map.MovePerson(person, "1", false);
            map.MovePerson(bob, "1", false);

            Registry registry = new Registry(new List<Person>() {
                person,
                bob
            });
            WorldState worldState = new WorldState(map, registry, WorldTime.DayZeroEightAM);

            ActionBuilder ab = new ActionBuilder(worldState, person);
            List<ActionBuilder.ActionData> actionData = ab.GatherProvidedActionsForActorAt("1");
            Assert.AreEqual(1, actionData.Count, string.Join(",", actionData));

            List<BoundAction> allActions = ab.GetAllActions("1");

            Assert.AreEqual(1, allActions.Count, string.Join(",", allActions));
            Assert.AreEqual(3, allActions[0].Bindings.Count);

            Assert.AreEqual("<a:alicia>,<b:person_bob>,<item:taco(3)>", string.Join(",", allActions[0].Bindings.bindings));
        }

        [Test]
        public void GenerateBoundActions_InventoryItem_Three()
        {
            Person person = new Person("alicia");
            Person bob = new Person("bob");


            Map map = new Map(
                new List<Feature>() {
                    //person.feature, //removed so there is only one feature in the room
                    bob.feature
                },
                new List<Location>() {
                    new Location("1", new Dictionary<string, List<string>>() {})
                }
            );
            person.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);
            bob.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);

            person.inventory.ChangeInventoryContents(3, "taco");
            person.inventory.ChangeInventoryContents(5, "ice");
            person.inventory.ChangeInventoryContents(1, "jade");
            bob.inventory.ChangeInventoryContents(2, "glass");

            map.MovePerson(person, "1", false);
            map.MovePerson(bob, "1", false);

            Registry registry = new Registry(new List<Person>() {
                person,
                bob
            });
            WorldState worldState = new WorldState(map, registry, WorldTime.DayZeroEightAM);

            ActionBuilder ab = new ActionBuilder(worldState, person);
            List<ActionBuilder.ActionData> actionData = ab.GatherProvidedActionsForActorAt("1");
            Assert.AreEqual(1, actionData.Count, string.Join(",", actionData));

            List<BoundAction> allActions = ab.GetAllActions("1");

            Assert.AreEqual(3, allActions.Count, string.Join(",", allActions));
            Assert.AreEqual(3, allActions[0].Bindings.Count);

            Assert.AreEqual("<give_taco(alicia, person_bob)>,<give_ice(alicia, person_bob)>,<give_jade(alicia, person_bob)>", string.Join(",", allActions), 
                allActions[0].ToString()+ string.Join(",", allActions[0].Bindings.bindings));

            Assert.AreEqual("<a:alicia>,<b:person_bob>,<item:taco(3)>", string.Join(",", allActions[0].Bindings.bindings));
        }
    }
}