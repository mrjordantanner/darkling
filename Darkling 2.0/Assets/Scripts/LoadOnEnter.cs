using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnEnter : StateMachineBehaviour {

    public int sceneIndex = 2;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.Instance.Initialize();
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(1);
	}

}
