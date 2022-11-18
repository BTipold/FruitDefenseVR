using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpgradeOrEnhancement : MonoBehaviour
{
    static bool UpgradeOrEnhancement = false;
    public void setUpOrEnhance(bool input)
    {
        UpgradeOrEnhancement = input;
    }
}
