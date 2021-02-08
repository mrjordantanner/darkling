using UnityEngine;

public class StartWaveButton : MonoBehaviour
{
    public void ExitMenuAndStart()
    {
        UpgradeController.Instance.CloseUpgradeMenu();
        WaveController.Instance.SetupNextWave();
    }

}
