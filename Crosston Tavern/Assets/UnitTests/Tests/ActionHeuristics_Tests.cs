using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ActionHeuristics_Tests
    {
        // A Test behaves as an ordinary method
    //    [Test]
    //    public void ActionHeuristics_SimpleCount()
    //    {
    //        Person person = new Person("alicia");
    //        Person bob = new Person("bob");


    //        Map map = new Map(
    //            new List<Feature>() {
    //                //person, //removed so there is only one feature in the room
    //                bob
    //            },
    //            new List<Location>() {
    //                new Location("1", new Dictionary<string, List<string>>() {})
    //            }
    //        );
    //        person.providedActions.Add(ActionInitializer.actions["give_#item#"]);
    //        bob.providedActions.Add(ActionInitializer.actions["give_#item#"]);

    //        person.inventory.ChangeInventoryContents(3, "taco");
    //        person.inventory.ChangeInventoryContents(5, "ice");
    //        person.inventory.ChangeInventoryContents(1, "jade");
    //        bob.inventory.ChangeInventoryContents(2, "glass");




    //        map.MovePerson(person.Id, "1", false);
    //        map.MovePerson(bob.Id, "1", false);

    //        Registry registry = new Registry(new List<Person>() {
    //            person,
    //            bob
    //        });
    //        WorldState worldState = new WorldState(map, registry, WorldTime.DayZeroEightAM);


    //        person.gm = new GoalManager(worldState, person);
    //        person.gm.AddModule(new GoalModule(new List<GM_Precondition>(), new List<Goal>() {
    //            new Goal(new StateInventory("alicia", "taco", 0, 1), 1)
    //        }));

    //        ActionBuilder ab = new ActionBuilder(worldState, person);
    //        List<BoundAction> allActions = ab.GetAllActions("1");

    //        ActionHeuristicManager ahm = new ActionHeuristicManager(person, worldState);
    //        List<WeightedAction> actions = ahm.WeighActions(allActions);

    //        Assert.AreEqual(3, actions.Count, string.Join(",", actions));
    //    }

    //    [Test]
    //    public void ActionHeuristics_CheckRational()
    //    {
    //        Person_old person = new Person_old("alicia");
    //        Person_old bob = new Person_old("bob");


    //        Map map = new Map(
    //            new List<Feature>() {
    //                //person.feature, //removed so there is only one feature in the room
    //                bob.feature
    //            },
    //            new List<Location>() {
    //                new Location("1", new Dictionary<string, List<string>>() {})
    //            }
    //        );
    //        person.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);
    //        bob.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);

    //        person.inventory.ChangeInventoryContents(3, "taco");

    //        map.MovePerson(person, "1", false);
    //        map.MovePerson(bob, "1", false);

    //        Registry registry = new Registry(new List<Person_old>() {
    //            person,
    //            bob
    //        });
    //        WorldState worldState = new WorldState(map, registry, WorldTime.DayZeroEightAM);


    //        person.gm = new GoalManager(worldState, person);
    //        person.gm.AddModule(new GoalModule(new List<GM_Precondition>(), new List<Goal>() {
    //            new Goal(new StateInventory("alicia", "taco", 0, 1), 1)
    //        }));

    //        ActionBuilder ab = new ActionBuilder(worldState, person);
    //        List<BoundAction> allActions = ab.GetAllActions("1");
    //        Assert.AreEqual(3, allActions[0].Bindings.bindings.Count, string.Join(",", allActions[0].Bindings.bindings));

    //        ActionHeuristicManager ahm = new ActionHeuristicManager(person, worldState);
    //        List<WeightedAction> actions = ahm.WeighActions(allActions);

    //        Assert.AreEqual(1, actions.Count, string.Join(",", actions));

    //        Assert.AreEqual(1, actions[0].weightRationals.Count, actions[0].VerboseString());
    //        Assert.Greater(actions[0].weight, 0, actions[0].ToString());
    //    }

    //    [Test]
    //    public void ActionHeuristics_ChooseBest()
    //    {
    //        Person_old person = new Person_old("alicia");
    //        Person_old bob = new Person_old("bob");


    //        Map map = new Map(
    //            new List<Feature>() {
    //                //person.feature, //removed so there is only one feature in the room
    //                bob.feature
    //            },
    //            new List<Location>() {
    //                new Location("1", new Dictionary<string, List<string>>() {})
    //            }
    //        );
    //        person.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);
    //        bob.feature.providedActions.Add(ActionInitializer.actions["give_#item#"]);

    //        person.inventory.ChangeInventoryContents(3, "taco");

    //        map.MovePerson(person, "1", false);
    //        map.MovePerson(bob, "1", false);

    //        Registry registry = new Registry(new List<Person_old>() {
    //            person,
    //            bob
    //        });
    //        WorldState worldState = new WorldState(map, registry, WorldTime.DayZeroEightAM);


    //        person.gm = new GoalManager(worldState, person);
    //        person.gm.AddModule(new GoalModule(new List<GM_Precondition>(), new List<Goal>() {
    //            new Goal(new StateInventory("alicia", "taco", 0, 1), 1)
    //        }));

    //        ActionHeuristicManager ahm = new ActionHeuristicManager(person, worldState);
    //        ChosenAction action = ahm.ChooseBestAction();

    //    }

    }
}
