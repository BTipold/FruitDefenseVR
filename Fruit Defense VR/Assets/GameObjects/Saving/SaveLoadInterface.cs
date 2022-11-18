using CommonTypes;
using System.Collections;
using System.Collections.Generic;

// NOTE: Save manager only exists in the context of games. Not in the mainmenu, or loading scene.

public class SaveLoadInterface
{
    private string mCurrentGameDatabaseId;
    private SaveLoadBackend mBackend;

    public SaveLoadInterface()
    {
        mBackend = SaveLoadBackend.GetInstance();
        string map = GameState.GetMap().ToString();
        string difficulty = GameState.GetDifficulty().ToString();
        mCurrentGameDatabaseId = map + difficulty;
    }

    public void Save()
    {
        mBackend.SaveDatabase(mCurrentGameDatabaseId);
    }

    public void Load()
    {
        mBackend.LoadDatabase(mCurrentGameDatabaseId);
    }

    public void RemoveCurrentSave()
    {
        mBackend.DeleteDatabase(mCurrentGameDatabaseId);
    }

    public void RegisterSaveableObject(SaveableObject obj)
    {
        mBackend.RegisterSaveableObject(obj, mCurrentGameDatabaseId);
    }

    public void UnRegisterSaveableObject(SaveableObject obj)
    {
        mBackend.UnRegisterSaveableObject(obj, mCurrentGameDatabaseId);
    }

    public static bool SaveFileExists(Map map, Difficulty difficulty)
    {
        string databaseId = map.ToString() + difficulty.ToString();
        return SaveLoadBackend.GetInstance().DatabaseExists(databaseId);
    }
}
