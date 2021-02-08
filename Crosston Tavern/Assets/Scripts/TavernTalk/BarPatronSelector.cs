using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPatronSelector
{

    //get all valid townies (probably a subset of whole town)
    //check to see who has goals they can't fulfill
    //check to see who has witnessed an action that would fullfill that goal

    List<string> validPatronNames = new List<string>();

    List<Townie> allPeople;
    WorldState ws;


    List<string> unvisited = new List<string>();

    public BarPatronSelector(List<Townie> allPeople, WorldState ws, List<string> validPatronNames)
    {
        this.allPeople = allPeople;
        this.ws = ws;
        this.validPatronNames = validPatronNames;
        NewNight();
    }

    public void NewNight()
    {
        unvisited = new List<string>(validPatronNames);
    }


    public void Visit(string patron)
    {
        if(unvisited.Contains(patron))
            unvisited.Remove(patron);
    }

    public Townie PickRandomPatron()
    {
        if (unvisited.Count == 0) return null;

        List<Townie> validPatrons = new List<Townie>();
        foreach (Townie townie in allPeople) {
            if (unvisited.Contains(townie.name)) {
                validPatrons.Add(townie);
            }
        }



        Townie chosen = validPatrons[Random.Range(0, validPatrons.Count)];

        return chosen;
    }

    public Townie PickNextPatron()
    {
        List<Townie> validPatrons = new List<Townie>();
        foreach(Townie townie in allPeople) {
            if (validPatronNames.Contains(townie.name)) {
                validPatrons.Add(townie);
            }
        }

        Dictionary<Townie, List<KeyValuePair<WeightedAction, WeightedAction>>> townieDesirableUnknownActions = GetAllUnknownActions(validPatrons);

        foreach(List<KeyValuePair<WeightedAction, WeightedAction>> townieActionSet in townieDesirableUnknownActions.Values) {
            List<WorldFact> learnableFacts = new List<WorldFact>();
            foreach (KeyValuePair<WeightedAction, WeightedAction> actionPair in townieActionSet) {
                WeightedAction action = actionPair.Key;
                string featureId = action.Bindings.BindString(action.FeatureId);

                List<WorldFact> relatedFacts = DecomposeActionsToKnowledge(action);
                foreach(WorldFact fact in relatedFacts) {
                    Debug.Log(fact);
                }


                //foreach (Outcome outcome in actionPair.Key.potentialOutcomes) {
                //    foreach (Effect effect in outcome.effects) {
                //        if (effect is EffectKnowledge) {
                //            EffectKnowledge knowledgeEffect = (EffectKnowledge)effect;
                //            WorldFact fact = knowledgeEffect.fact;
                //            fact = fact.Bind(actionPair.Key.Bindings, ws.map.GetFeature(featureId).relevantResources);

                //            if(fact is WorldFactResource) {
                //                WorldFactResource factResource = (WorldFactResource)fact;
                                

                //            }

                //        }
                //    }
                    
                //}
            }
        }


        return null;
    }

    /// <summary>
    /// Get the actions that a given townie would like to know more about
    /// </summary>
    /// <param name="validPatrons"></param>
    /// <returns>
    /// key: townie
    /// value: list of: 
    ///     key: action according to world data
    ///     value: action according to townie's knowledge
    /// </returns>
    private Dictionary<Townie, List<KeyValuePair<WeightedAction, WeightedAction>>> GetAllUnknownActions(List<Townie> validPatrons)
    {
        Dictionary<Townie, List<KeyValuePair<WeightedAction, WeightedAction>>> townieDesirableUnknownActions =
                                    new Dictionary<Townie, List<KeyValuePair<WeightedAction, WeightedAction>>>();
        foreach (Townie townie in validPatrons) {

            // Get goals
            List<Goal> goals = townie.townieInformation.knownGoals;

            // Get all actions they know of
            ActionBuilder ab = new ActionBuilder(townie.ws, townie.townieInformation);
            List<BoundAction> knownPotentialActions = ab.GetAllActions(respectLocation: false);
            ActionHeuristicManager ahm = new ActionHeuristicManager(townie.townieInformation, townie.ws);
            List<WeightedAction> knownWeightedActions = ahm.WeighActions(knownPotentialActions, false);


            // Get all actions across world
            ab = new ActionBuilder(ws, townie.townieInformation);
            List<BoundAction> allPotentialActions = ab.GetAllActions(respectLocation: false);
            ahm = new ActionHeuristicManager(townie.townieInformation, ws);
            List<WeightedAction> allWeightedActions = ahm.WeighActions(allPotentialActions, false);

            List<KeyValuePair<WeightedAction, WeightedAction>> zipped = new List<KeyValuePair<WeightedAction, WeightedAction>>();
            List<WeightedAction> unpaired = new List<WeightedAction>();
            foreach (WeightedAction fromAll in allWeightedActions) {
                WeightedAction pair = null;
                foreach (WeightedAction fromKnown in knownWeightedActions) {
                    if (fromAll.ToId() == fromKnown.ToId()) {
                        pair = fromKnown;
                        break;
                    }
                }
                if (pair != null)
                    zipped.Add(new KeyValuePair<WeightedAction, WeightedAction>(fromAll, pair));
                else
                    unpaired.Add(fromAll);
            }

            List<KeyValuePair<WeightedAction, WeightedAction>> wouldLikeToLearnAbout = new List<KeyValuePair<WeightedAction, WeightedAction>>();
            Debug.Log(townie.townieInformation.id);
            foreach (WeightedAction action in unpaired) {
                //Debug.Log(action);
                if (action.weight > 0) wouldLikeToLearnAbout.Add(new KeyValuePair<WeightedAction, WeightedAction>(action, null));
            }
            foreach (KeyValuePair<WeightedAction, WeightedAction> pair in zipped) {
                Debug.Log(pair.Key + "-" + pair.Value);
                if (pair.Key.weight > pair.Value.weight && pair.Value.weight > 0) wouldLikeToLearnAbout.Add(pair);
            }

            townieDesirableUnknownActions.Add(townie, wouldLikeToLearnAbout);

            //Debug.Log("OF INTEREST:");
            foreach(KeyValuePair<WeightedAction, WeightedAction> action in wouldLikeToLearnAbout) {
                Debug.Log(action.Key);
            }
        }

        return townieDesirableUnknownActions;
    }



    private List<WorldFact> DecomposeActionsToKnowledge(WeightedAction action)
    {
        List<WorldFact> relatedKnowledge = new List<WorldFact>();


        BoundBindingCollection bindings = action.Bindings;
        string actorId = bindings.BindString(action.ActorId);
        string featureId = bindings.BindString(action.FeatureId);
        FeatureResources resources = ws.map.GetFeature(featureId).relevantResources;

        foreach(BoundBindingPort binding in bindings.bindings) {
            
            string resourceType = binding.Value.Trim('#');

            if (resources.resources.ContainsKey(resourceType)) {
                List<string> posibleResources = resources.BindString(binding.Value);

                foreach (string str in posibleResources) {
                    relatedKnowledge.Add(new WorldFactResource(featureId, resourceType, str));
                }
            }
        }

        foreach(Outcome outcome in action.potentialOutcomes) {
            ChanceModifier chanceModifier = outcome.chanceModifier;
            if(chanceModifier is ChanceModifierItemOpinion) {
                ChanceModifierItemOpinion itemOpnion = (ChanceModifierItemOpinion)chanceModifier;
                itemOpnion = (ChanceModifierItemOpinion)itemOpnion.MakeBound(bindings, resources);
                string item = itemOpnion.item;
                string person = itemOpnion.person;
                PreferenceLevel level = ws.registry.GetPerson(person).ItemPreference(item);

                relatedKnowledge.Add(new WorldFactPreference(person, level, item));

            }
        }


        return relatedKnowledge;
    }
}
