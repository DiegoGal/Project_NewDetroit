using UnityEngine;
using System.Collections;

// recoge los cambios de estados de las unidades instanciadas por red
// y en función de estos cambios cambia las animaciones de la unidad
public class UnitAnimationsNetwork : MonoBehaviour
{

    // modelo del asset (el que contiene las animaciones)
    protected Transform model;

    void Awake ()
    {
        model = transform.FindChild("Model");
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void ChangeAnimation (UnitController.State nextState)
    {
        switch (nextState)
        {
            case UnitController.State.Idle:           animation.CrossFade("Idle01");  break;
            case UnitController.State.GoingTo:        animation.CrossFade("Walk");    break;
            case UnitController.State.GoingToAnEnemy: animation.CrossFade("Walk");    break;
            case UnitController.State.Attacking:      animation.CrossFade("Attack1"); break;
            case UnitController.State.Dying:          animation.CrossFade("Die");     break;
            case UnitController.State.Flying: break;
            case UnitController.State.AscendingToHeaven:
                // TODO! cambiar el material de la unidad

                // update the Alpha Multiply Value of the material
                float alphaValue = model.renderer.material.GetFloat("_AlphaMultiplyValue");
                alphaValue *= 0.97f;
                alphaValue -= 0.006f;
                model.renderer.material.SetFloat("_AlphaMultiplyValue", alphaValue);
                if (alphaValue <= 0.0f)
                    Destroy(this.gameObject);
                break;
        }
    }

}
