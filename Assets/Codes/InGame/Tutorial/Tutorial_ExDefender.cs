using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_ExDefender : MonoBehaviour
{
    public Tutorial Tutorial;
    public Defender Defender;

    public enum TutoSequence
    {
        Tuto_1, Tuto_2, Tuto_3, Tuto_4, Tuto_5, Tuto_6, None
    }

    public TutoSequence tutoNum;

    bool isSetTuto;

    void Update()
    {
        if (!isSetTuto)
        {
            switch (tutoNum)
            {
                case TutoSequence.Tuto_1:
                    if (Defender.isTackle && transform.position.z - Player.Instance.transform.position.z <= 25)
                    {
                        isSetTuto = true;
                        Tutorial.TutoralSet((int)TutoSequence.Tuto_1);
                    }
                    break;

                case TutoSequence.Tuto_2:
                    if (Defender.isTackle && Defender.stateInfo.IsName("Tackle_Front"))
                    {
                        isSetTuto = true;
                        Tutorial.TutoralSet((int)TutoSequence.Tuto_2);
                    }
                    break;
                    
                case TutoSequence.Tuto_3:
                    if (Defender.isTackle && Defender.stateInfo.IsName("Tackle_Right"))
                    {
                        isSetTuto = true;
                        Tutorial.TutoralSet((int)TutoSequence.Tuto_3);
                    }
                    break;

                case TutoSequence.Tuto_5:
                    if (Defender.isTackle && transform.position.z - Player.Instance.transform.position.z <= 25)
                    {
                        isSetTuto = true;
                        Tutorial.TutoralSet((int)TutoSequence.Tuto_5);
                    }
                    break;

                case TutoSequence.Tuto_6:
                    if (Defender.isTackle)
                    {
                        isSetTuto = true;
                        Tutorial.TutoralSet((int)TutoSequence.Tuto_6);
                    }
                    break;
            }
        }

        
    }
}
