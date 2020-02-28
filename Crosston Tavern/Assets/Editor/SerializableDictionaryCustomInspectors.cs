using UnityEditor;

[CustomPropertyDrawer(typeof(StringListStorage))]
public class StringListStoragePropertyDrawer :
SerializableDictionaryStoragePropertyDrawer
{ }
[CustomPropertyDrawer(typeof(StringStringListDictionary))]
[CustomPropertyDrawer(typeof(StringIntDictionary))]
[CustomPropertyDrawer(typeof(StringFloatDictionary))]
public class AnyDictionaryPropertyDrawer :
SerializableDictionaryPropertyDrawer
{ }