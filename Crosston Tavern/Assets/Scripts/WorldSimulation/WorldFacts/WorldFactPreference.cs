using System.Collections.Generic;

[System.Serializable]
public class WorldFactPreference: WorldFact
{
    public string person;
    public PreferenceLevel level;
    public string item;

    public WorldFactPreference(string person, PreferenceLevel level, string item)
    {
        this.person = person;
        this.level = level;
        this.item = item;

        id = ToString();
    }

    public override string ToString()
    {
        return "{" + person + ":(" + level.ToString() + "," + item + ")}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactPreference)) return false;

        WorldFactPreference fact = (WorldFactPreference)obj;
        if (this.person == fact.person &&
            this.level == fact.level &&
            this.item == fact.item) return true;

        return false;
    }

    public override WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string person = bindings.BindString(this.person);
        string item = bindings.BindString(this.item);

        return new WorldFactPreference(person, this.level, item);
    }

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {
        ItemPreference pref = ws.map.GetPerson(person).preference;

        pref.Add(item, level);

        return base.UpdateWorldState(ws);
    }

    public override int GetHashCode()
    {
        var hashCode = 1523044811;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(person);
        hashCode = hashCode * -1521134295 + level.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(item);
        return hashCode;
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        string level;
        string name = person;

        if (this.level == PreferenceLevel.neutral) {
            if (name == speaker || name == listener) level = "don't care one way or the other about";
            else level = "doesn't care one way or the other about";
        }else if (this.level == PreferenceLevel.disliked) {
            if (name == speaker || name == listener) level = "don't really like";
            else level = "doesn't really like";
        } else {
            level = this.level.ToString();
            level = level.Remove(level.Length - 1, 1);
            if (name != speaker && name != listener) level += "s";
        }

        if (name == speaker) name = "I";
        if (name == listener) name = "you";

        name = VerbalizationDictionary.Replace(name);
        string item = VerbalizationDictionary.Replace(this.item, VerbalizationDictionary.Form.general);

        return name + " " + level + " " + item;
    }
}
