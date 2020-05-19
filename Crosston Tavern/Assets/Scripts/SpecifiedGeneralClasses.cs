using System;
using System.Collections.Generic;


[Serializable]
public class StringListStorage : SerializableDictionary.Storage<List<string>>
{ }
[Serializable]
public class StringStringListDictionary : SerializableDictionary<string, List<string>, StringListStorage>
{ }[Serializable]public class PreferencesDictionary : SerializableDictionary<PreferenceLevel, List<string>, StringListStorage>
{ }[Serializable]public class FloatArrayStorage : SerializableDictionary.Storage<float[]> { }[Serializable]public class StringFloatArrayDictionary : SerializableDictionary<string, float[], FloatArrayStorage> { }

[Serializable]
public class StringIntDictionary: SerializableDictionary<string, int> { }
[Serializable]
public class StringFloatDictionary : SerializableDictionary<string, float> { }
[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> { }

[Serializable]
public class GoalDictionary : SerializableDictionary<Effect, float> { }