using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    // 애니메이션 컨트롤러
    [SerializeField] RuntimeAnimatorController allyController;
    [SerializeField] RuntimeAnimatorController enemyController;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void ApplyAnime()
    {
        bool isAlly = gameObject.layer == (int)Layer.Player;
        animator.runtimeAnimatorController = isAlly ? allyController : enemyController;
    }
}
