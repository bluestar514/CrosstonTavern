using UnityEditor;

[CustomPropertyDrawer(typeof(StringListStorage))]
[CustomPropertyDrawer(typeof(FloatArrayStorage))]
public class StringListStoragePropertyDrawer :
SerializableDictionaryStoragePropertyDrawer
{ }
[CustomPropertyDrawer(typeof(StringStringListDictionary))]
[CustomPropertyDrawer(typeof(StringFloatArrayDictionary))]
[CustomPropertyDrawer(typeof(StringIntDictionary))]
[CustomPropertyDrawer(typeof(StringFloatDictionary))]
[CustomPropertyDrawer(typeof(GoalDictionary))]
public class AnyDictionaryPropertyDrawer :
SerializableDictionaryPropertyDrawer
{ }