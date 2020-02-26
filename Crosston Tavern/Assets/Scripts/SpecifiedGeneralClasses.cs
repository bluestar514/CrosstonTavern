using System;
using System.Collections.Generic;


[Serializable]
public class StringListStorage : SerializableDictionary.Storage<List<string>>
{ }

[Serializable]
public class StringStringListDictionary : SerializableDictionary<string,
List<string>, StringListStorage>
{ }
