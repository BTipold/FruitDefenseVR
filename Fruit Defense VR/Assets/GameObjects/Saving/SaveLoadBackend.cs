// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System.Collections.Generic;


public class SaveLoadBackend
{
    public static SaveLoadBackend Instance = null;
    private Dictionary<string, Database> mDatabases = new Dictionary<string, Database>();

    // -----------------------------------------------------------
    // @Summary: Singleton instance getter. Creates the singleton
    //   if not called yet. Otherwise returns the singleton. 
    // @Return: SaveLoadBackend - returns the singleton instance.
    // -----------------------------------------------------------
    public static SaveLoadBackend GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SaveLoadBackend();
        }
        return Instance;
    }

    // -----------------------------------------------------------
    // @Summary: Registers saveable object to specified database.
    //   When registered, a SaveDatabase() call will save the 
    //   item (as well as all other registered items).
    // @Param: obj - SaveableObject to register.
    // @Param: databaseId - Name/ID of the save group database.
    // @Return: void
    // -----------------------------------------------------------
    public void RegisterSaveableObject(SaveableObject obj, string databaseId)
    {
        if (!mDatabases.ContainsKey(databaseId))
        {
            mDatabases.Add(databaseId, new Database(databaseId));
        }
        mDatabases[databaseId].RegisterItem(obj);
    }

    // -----------------------------------------------------------
    // @Summary: Unregisters saveable object to specified
    //   database.
    // @Param: obj - SaveableObject to unregister.
    // @Param: databaseId - Name/ID of the save group database.
    // @Return: void
    // -----------------------------------------------------------
    public void UnRegisterSaveableObject(SaveableObject obj, string databaseId)
    {
        if (mDatabases.ContainsKey(databaseId))
        {
            mDatabases[databaseId].UnRegisterItem(obj);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Saves all registered items in the database.
    // @Param: databaseId - Name/ID of the save group database.
    // @Return: void
    // -----------------------------------------------------------
    public void SaveDatabase(string databaseId)
    {
        if (mDatabases.ContainsKey(databaseId))
        {
            mDatabases[databaseId].SaveAll();
        }
    }

    // -----------------------------------------------------------
    // @Summary: Loads all registered items in the database.
    // @Param: databaseId - Name/ID of the save group database.
    // @Return: void
    // -----------------------------------------------------------
    public void LoadDatabase(string databaseId)
    {
        if (mDatabases.ContainsKey(databaseId))
        {
            mDatabases[databaseId].LoadAll();
        }
    }

    // -----------------------------------------------------------
    // @Summary: Deletes database and all save files in it.
    // @Param: databaseId - Name/ID of the save group database.
    // @Return: void
    // -----------------------------------------------------------
    public void DeleteDatabase(string databaseId)
    {
        if (mDatabases.ContainsKey(databaseId))
        {
            mDatabases[databaseId].DeleteDatabase();
            mDatabases.Remove(databaseId);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Saves raw bytes to the specified database. Since
    //   it's not registered, it won't be saved when SaveDatabase
    //   is called.
    // @Param: data - raw data to save.
    // @Param: key - keyname to save data under.
    // @Param: databaseId - Name/ID of the save group database.
    // @Return: void
    // -----------------------------------------------------------
    public void Save(byte[] data, string key, string databaseId)
    {
        if (!mDatabases.ContainsKey(databaseId))
        {
            mDatabases.Add(databaseId, new Database(databaseId));
        }
        mDatabases[databaseId].Save(data, key);
    }

    // -----------------------------------------------------------
    // @Summary: Saves raw bytes to the specified database. Since
    //   it's not registered, it won't be saved when SaveDatabase
    //   is called.
    // @Param: key - keyname used to retreive data from.
    // @Param: valid - output param to indicate failures.
    // @Param: databaseId - Name/ID of the save group database.
    // @Return: byte[] - returns the loaded bytes
    // -----------------------------------------------------------
    public byte[] Load(string key, out bool valid, string databaseId)
    {
        if (!mDatabases.ContainsKey(databaseId))
        {
            mDatabases.Add(databaseId, new Database(databaseId));
        }

        byte[] data = mDatabases[databaseId].Load(key, out valid);
        return data;
    }

    // -----------------------------------------------------------
    // @Summary: checks if database exists.
    // @Param: name - the name of database to check if exists.
    // @Return: bool - returns true if database exists.
    // -----------------------------------------------------------
    public bool DatabaseExists(string name)
    {
        return mDatabases.ContainsKey(name);
    }
}
