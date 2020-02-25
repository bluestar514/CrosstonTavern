using UnityEditor;

[CustomPropertyDrawer(typeof(StringListStorage))]
public class StringListStoragePropertyDrawer :
SerializableDictionaryStoragePropertyDrawer
{ }
[CustomPropertyDrawer(typeof(StringStringListDictionary))]
public class StringStringListStoragePropertyDrawer :
SerializableDictionaryPropertyDrawer
{ }