// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Database
{
    private string mDatabaseName = null;
    private string mDatabasePath = null;
    private Dictionary<string, SaveableObject> mRegisteredObjects = new Dictionary<string, SaveableObject>();

    // -----------------------------------------------------------
    // @Summary: Constructor.
    // @Param: string - database name refers to the folder where
    //   the data will be stored. 
    // @Return: void
    // -----------------------------------------------------------
    public Database(string databaseName)
    {
        mDatabaseName = databaseName;
        mDatabasePath = Application.persistentDataPath + "/.persistantdata/" + databaseName;
        CreateDirectory();
    }

    // -----------------------------------------------------------
    // @Summary: registers item to the database so that when the
    //   database is saved, this item will be saved with it. If 
    //   items are not registered, they will only be saved when
    //   called directly from the SaveableObject.
    // @Param: obj - SaveableObject to be registered.
    // @Return: void
    // -----------------------------------------------------------
    public void RegisterItem(SaveableObject obj)
    {
        mRegisteredObjects.Add(obj.GetKey(), obj);
    }

    // -----------------------------------------------------------
    // @Summary: Unregisters the saveable object.
    // @Param: obj - SaveableObject to be unregistered.
    // @Param: erase - if true, will erase the object as well as 
    //   unregister it. 
    // @Return: void
    // -----------------------------------------------------------
    public void UnRegisterItem(SaveableObject obj, bool erase=false)
    {
        if(mRegisteredObjects.ContainsKey(obj.GetKey()))
        {
            if (erase) DeleteItem(obj.GetKey());
            mRegisteredObjects.Remove(obj.GetKey());
        }
    }

    // -----------------------------------------------------------
    // @Summary: Saves the object directly to the database.
    // @Param: obj - SaveableObject to be saved.
    // @Return: void
    // -----------------------------------------------------------
    public void Save(SaveableObject obj)
    {
        if (!obj.IsDataNull())
        {
            byte[] binaryData = obj.SerializeData();

            // encryptedData = Encrypt(binarydata, password);
            WriteToDisk(binaryData, obj.GetKey());

        } else {
            Debug.LogError("cannot save " + obj.ToString() + " because data is null");
        }
    }

    // -----------------------------------------------------------
    // @Summary: Load the saveable object.
    // @Param: obj - saveable object to restore.
    // @Return: void
    // -----------------------------------------------------------
    public void Load(SaveableObject obj)
    {
        var bytes = LoadFromDisk(obj.GetKey(), out bool valid);
        Debug.Log((valid ? "SUCCESS" : "FAIL") + " loading " + obj.GetKey());
        if (valid) {
            // decryptedData = Decrypt(binarydata, password);
            obj.UnSerializeData(bytes);
            obj.OnLoad();
        } else {
            obj.OnLoadFailed();
        }
    }

    // -----------------------------------------------------------
    // @Summary: Saves a stream of bytes, instead of a 
    //   SaveableObject. 
    // @Param: bytes - byte stream to save. 
    // @Param: key - key used to save the data.
    // @Return: void
    // -----------------------------------------------------------
    public void Save(byte[] bytes, string key)
    {
        // encryptedData = Encrypt(binarydata, password);
        WriteToDisk(bytes, key);
    }

    // -----------------------------------------------------------
    // @Summary: Loads a stream of bytes, instead of a 
    //   SaveableObject. 
    // @Param: key - key used to save the data.
    // @Param: valid - output bool indicating if load was
    //   successful. 
    // @Return: byte[] - returns stream of bytes from save file
    // -----------------------------------------------------------
    public byte[] Load(string key, out bool valid)
    {
        Debug.Log("Loading " + key);
        return LoadFromDisk(key, out valid);
    }

    // -----------------------------------------------------------
    // @Summary: Saves all the saveable objects that were 
    //   registered to this database. 
    // @Return: void
    // -----------------------------------------------------------
    public void SaveAll()
    {
        foreach (SaveableObject obj in mRegisteredObjects.Values)
        {
            Save(obj);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Loads all the saveable objects in the database
    //   and restores the data to the SaveableObject.
    // @Return: void
    // -----------------------------------------------------------
    public void LoadAll()
    {
        foreach (SaveableObject obj in mRegisteredObjects.Values)
        {
            Load(obj);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Returns the database name
    // @Return: string - returns database name as a string
    // -----------------------------------------------------------
    public string GetName()
    {
        return mDatabaseName;
    }

    // -----------------------------------------------------------
    // @Summary: Delete item from disk.
    // @Param: key - name of file to remove.
    // @Return: void
    // -----------------------------------------------------------
    public void DeleteItem(string key)
    {
        var f = mDatabasePath + "/" + key;
        if (File.Exists(f))
        {
            File.Delete(f);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Creates the directory with database name. The
    //   directory will hold all persistent files saved to the 
    //   database.
    // @Return: void
    // -----------------------------------------------------------
    public void CreateDirectory()
    {
        Directory.CreateDirectory(mDatabasePath);
    }

    // -----------------------------------------------------------
    // @Summary: Removes the entire directory and all saved files
    //   inside it. 
    // @Return: void
    // -----------------------------------------------------------
    public void DeleteDatabase()
    {
        Directory.Delete(mDatabasePath, true);
    }

    // -----------------------------------------------------------
    // @Summary: Writes raw bytes to disk with the key as the 
    //   filename. 
    // @Param: data - raw bytes to save.
    // @Param: name - filename to save data under.
    // @Return: void
    // -----------------------------------------------------------
    public void WriteToDisk(byte[] data, string name)
    {
        using (FileStream fs = File.Create(mDatabasePath + "/" + name))
        {
            fs.Write(data);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Loads raw bytes from file on disk.
    // @Param: name - filename to load data from.
    // @Param: valid - output param indicating whether load was 
    //   successful.
    // @Return: void
    // -----------------------------------------------------------
    public byte[] LoadFromDisk(string name, out bool valid)
    {
        valid = File.Exists(mDatabasePath + "/" + name);
        return valid ? File.ReadAllBytes(mDatabasePath + "/" + name) : null;
    }
}
