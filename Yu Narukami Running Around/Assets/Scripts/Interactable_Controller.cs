using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable_Controller : MonoBehaviour
{
    public enum characterExpressions
    {
        Neutral = 0,
        Glad = 1,
        Happy = 2,
        Sad = 3,
        Shocked = 4,
        Worried = 5,
        Meh = 6,
        MAX
    };
    [Header ("Dialogue Fields")]
	[SerializeField] private string dialoguePath;
	[SerializeField] private bool isDialogueInDaily;
	[SerializeField] private bool isDialoguePhone;
    [Header ("Portrait Fields")]
    [SerializeField] private bool containsCharacterPortrait;
    [SerializeField] private Sprite characterPortrait;
    [SerializeField] private Sprite characterPortraitBG;
    [SerializeField] private string characterName;
    [SerializeField] private AnimatorOverrider characterPortraitAnimator;

    [Header ("Mouth Animations")]
    [SerializeField] private AnimatorOverrideController neutralAnimation;
    [SerializeField] private AnimatorOverrideController gladAnimation;
    [SerializeField] private AnimatorOverrideController happyAnimation;
    [SerializeField] private AnimatorOverrideController sadAnimation;
    [SerializeField] private AnimatorOverrideController shockedAnimation;
    [SerializeField] private AnimatorOverrideController worriedAnimation;
    [SerializeField] private AnimatorOverrideController mehAnimation;
    
    private characterExpressions currentMood;
	private Dictionary<characterExpressions, AnimatorOverrideController> moodAnimation =
        new Dictionary<characterExpressions, AnimatorOverrideController>();
  

    void Start()
    {
        if(containsCharacterPortrait)
        {
            moodAnimation.Add(characterExpressions.Neutral, neutralAnimation);
            moodAnimation.Add(characterExpressions.Glad, 	gladAnimation);
            moodAnimation.Add(characterExpressions.Happy, 	happyAnimation);
            moodAnimation.Add(characterExpressions.Sad, 	sadAnimation);
            moodAnimation.Add(characterExpressions.Shocked, shockedAnimation);
            moodAnimation.Add(characterExpressions.Worried, worriedAnimation);
            moodAnimation.Add(characterExpressions.Meh, 	mehAnimation);
        }
    }

    void Update()
    {}

    public void startAnimation(characterExpressions newMood)
    {
        if(currentMood != newMood)
            currentMood = newMood;
        if(currentMood == characterExpressions.MAX)
            currentMood = characterExpressions.Neutral;

        characterPortraitAnimator.setAnimation(moodAnimation[currentMood]);
        Debug.Log(moodAnimation[currentMood]);
        characterPortraitAnimator.setTrigger("Talk");
        characterPortraitAnimator.setTrigger("Blink");
        Debug.Log("Play Animations|" + "Mood: " + currentMood);
    }

    public void stopAnimation()
    {
        characterPortraitAnimator.setTrigger("Neutral");
        //characterPortraitAnimator.setTrigger("Blink");
    }

    public void setDialoguePath(string path)                {dialoguePath = path;               }
    public string getDialoguePath()                         {return dialoguePath;               }
    public void setCharacterName(string path)               {characterName = path;               }
    public string getCharacterName()                        {return characterName;               }
    public void setCharacterPortrait(Sprite sprite)         {characterPortrait = sprite;        }
    public Sprite getCharacterPortrait()                    {return characterPortrait;          }
    public void setCharacterPortraitBG(Sprite sprite)       {characterPortraitBG = sprite;      }
    public Sprite getCharacterPortraitBG()                  {return characterPortraitBG;        }
    public void setContainsCharacterPortrait(bool value)    {containsCharacterPortrait = value; }
    public bool getContainsCharacterPortrait()              {return containsCharacterPortrait;  }
    public void setIsDialogueInDaily(bool value)            {isDialogueInDaily = value;         }
    public bool getIsDialogueInDaily()                      {return isDialogueInDaily;          }
    public void setIsDialogueInPhone(bool value)            {isDialoguePhone = value;           }
    public bool getIsDialogueInPhone()                      {return isDialoguePhone;            }
}
