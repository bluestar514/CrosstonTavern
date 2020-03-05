using UnityEditor;

[CustomPropertyDrawer(typeof(StringListStorage))]
public class StringListStoragePropertyDrawer :
SerializableDictionaryStoragePropertyDrawer
{ }
[CustomPropertyDrawer(typeof(StringStringListDictionary))]
[CustomPropertyDrawer(typeof(StringIntDictionary))]
[CustomPropertyDrawer(typeof(StringFloatDictionary))]
[CustomPropertyDrawer(typeof(GoalDictionary))]
public class AnyDictionaryPropertyDrawer :
SerializableDictionaryPropertyDrawer
{ }