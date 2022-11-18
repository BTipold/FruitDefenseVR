// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


public class PersistentDataType<T> : SaveableObject 
{
    private T mData;
    private SaveLoadBackend mBackend = null;
    private const string DEFAULT_DATABASE = "global";

    // -----------------------------------------------------------
    // @Summary: Constructor
    // @Param: value - data to save
    // @Param: key - unique identifier used to retreive the 
    //   item later
    // @Return: void
    // -----------------------------------------------------------
    public PersistentDataType(T value, string key) : base(key)
    {
        mData = value;
        mBackend = SaveLoadBackend.GetInstance();
    }

    // ---------------------------------------------------------------
    // @Summary: returns the true if underlying data is null
    // @Return: bool - returns null
    // ---------------------------------------------------------------
    public override bool IsDataNull() { return mData == null; }

    // -----------------------------------------------------------
    // @Summary: Assigns new data item
    // @Param: newVal - new data to set
    // @Return: void
    // -----------------------------------------------------------
    public void Assign(T newVal)
    {
        mData = newVal;
    }

    // -----------------------------------------------------------
    // @Summary: Saves the item directly, without registering to 
    //   save manager.
    // @Param: database - database to save this item in
    // @Return: void
    // -----------------------------------------------------------
    public void Save(string database=DEFAULT_DATABASE)
    {
        mBackend.Save(SerializeData(), mKey, database);
    }

    // -----------------------------------------------------------
    // @Summary: Loads the item directly, without being 
    //   registered to the save manager.
    // @Param: database - database to load item from
    // @Return: void
    // -----------------------------------------------------------
    public void Load(string database=DEFAULT_DATABASE)
    {
        var bytes = mBackend.Load(mKey, out bool valid, database);
        if (valid) {
            UnSerializeData(bytes);
        } else {
            OnLoadFailed();
        }
    }

    // -----------------------------------------------------------
    // @Summary: Converts the underlying type to saveable bytes.
    //   Underlying type MUST be marked as serializeable
    // @Return: byte[] - returns prefab as a stream of bytes
    // -----------------------------------------------------------
    public override byte[] SerializeData()
    {
        byte[] bytes;
        IFormatter serializer = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            serializer.Serialize(stream, mData);
            bytes = stream.ToArray();
        }
        return bytes;
    }

    // -----------------------------------------------------------
    // @Summary: Converts a data stream back to the original 
    //   underlying data object. 
    // @Param: data - serialized data 
    // @Return: void
    // -----------------------------------------------------------
    public override void UnSerializeData(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            IFormatter serializer = new BinaryFormatter();
            mData = (T)serializer.Deserialize(ms);
        }
    }

    // -----------------------------------------------------------
    // @Summary: implicit conversion from
    //   PersistentPrefab => GameObject
    // @Param: objRef - reference to the object being implicitly
    //   converted.
    // @Return: GameObject - returns underlying prefab
    // -----------------------------------------------------------
    public static implicit operator T(PersistentDataType<T> objRef) => objRef.mData;

    // -----------------------------------------------------------
    // @Summary: override ToString - use underlying toString
    // @Return: string - returns gameobject tostring
    // -----------------------------------------------------------
    public override string ToString()
    {
        return mKey;
    }

    // -----------------------------------------------------------
    // @Summary: override object equals
    // @Param: obj - ref to object being compared
    // @Return: bool - returns true if underlying gameobject is 
    //   equal to the input obj
    // -----------------------------------------------------------
    public override bool Equals(object obj)
    {
        var item = obj as PersistentDataType<T>;
        if (item == null) return false;
        return mData.Equals(item.mData);
    }

    // -----------------------------------------------------------
    // @Summary: override Hash method
    // @Return: int - returns hash of underlying gameobject
    // -----------------------------------------------------------
    public override int GetHashCode()
    {
        return mData.GetHashCode();
    }
}
