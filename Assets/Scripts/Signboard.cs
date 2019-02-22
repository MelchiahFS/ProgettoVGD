using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Signboard : MonoBehaviour {

	private bool boardActive = false;
	public GameObject signboard, inputHint, hidingPanel;
	public Text signboardText;

	public AudioClip open, close;
	private AudioSource source;

	private string[] levelBoards = {
		"You are Zedd, an apprentice Sorcerer in search of the Sacred Scroll. You need it to save your family from a curse that was " +
			"thrown at them by an evil Mage you once thought was a good master. When you discovered what kind of atrocity he committed with his obscure arts, " +
			"you refused to help him and fought against him. You somehow won the battle, but just before dying, he cast a spell that condemned your beloved ones " +
			"to vanish from this world, and become corrupted creatures. Only the Sacred Scroll can help them now...",
		"You knew it would have been hard to descend into this hell-pit. You need to be cautious, and to remember all your teachings. In every corner of this hellish " +
			"place possibly hides an abomination ready to feast on your body. Keep your eyes open.",
		"When your evil master, at the times he was teaching you the principles of magic, mentioned this obscure labyrinth for the first time, you noticed his voice " +
			"was trembling. Even with his power and experience, he never dared to enter this place. The price was too high, even for him. Now you know why. You " +
			"killed countless hoards of enemies, but their voices and screams are still echoing through the dark. And now other horrible creatures are coming...",
		"Strange thoughts appear on your mind. They tell you that it is good if you let them chop your head off. They tell you that you can find extreme pleasure in " +
			"death. But as you remember why you're here, you recognize that it's not your inner voice you're hearing. There's some sort of demon presence here. " +
			"Keep your mind calm, and remember to stay in control. Madness is knocking on your door, so don't let it in!",
		"You arrived this far, wounded and tired, with only force of will on your side. You can feel this is where your journey ends, for better or worse. " +
			"Malignity fills the putrid air you're breathing. But, even inside this obscure place, you can feel something that is... good. Benevolent. Friendly. " +
			"It is the Sacred Scrool that calls you. And finally you find new strength to stand up again and fight. There's still hope!",
		"With your hands covered in blood, you lift the Sacred Scroll from the altar where it magicly appeared. At the very moment when you touched its paper, " +
			"and posed your eyes on its glyphs, a burst of new energy flowed into your veins. And when you started reading the magic words, all the evil that " +
			"remained in this bolge dissipated instantly. You look at the Sacred Scroll as it becomes dust. Inside your mind you can now clearly hear the voices of" +
			" your family, thanking you for freeing them. Backing up to home, you think it's time to forget all this pain, and return to live happy.\r\n\r\nTHE END"
	};

	void Start()
	{
		source = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (!GameManager.manager.inventoryActive && !GameManager.manager.pauseMenuActive && !GameManager.manager.ending)
		{
			if (GameManager.manager.signboardContact)
			{
				if (Input.GetKeyDown(KeyCode.E))
				{
					if (!boardActive)
					{
						source.PlayOneShot(open);
						boardActive = true;
						signboard.SetActive(true);
						inputHint.SetActive(true);
						GameManager.manager.signboardActive = true;
						GameManager.manager.gamePause = true;
						Time.timeScale = 0;

						//DA FARE: CREARE TESTO PER OGNI LIVELLO E PER FINE GIOCO ---------------------------------------------
						if (GameManager.manager.ActualRoom.startRoom)
						{
							signboardText.text = levelBoards[GameStats.stats.levelNumber - 1];
							
						}
						else if (GameManager.manager.ActualRoom.bossRoom)
						{
							signboardText.text = levelBoards[GameStats.stats.levelNumber];
						}
						Canvas.ForceUpdateCanvases();
					}
					else
					{
						source.PlayOneShot(close);
						boardActive = false;
						signboard.SetActive(false);
						inputHint.SetActive(false);
						GameManager.manager.signboardActive = false;
						GameManager.manager.gamePause = false;
						
						if (GameStats.stats.levelNumber == 5 && GameManager.manager.ActualRoom.bossRoom)
						{
							if (!GameManager.manager.ending)
							{
								Destroy(GameStats.stats.gameObject);
								StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "Menu"));
							}
								
						}

						Time.timeScale = 1;
					}
				}
			}
		}
	}

}
