using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Example.Common
{
    public class PerkUpgradeButton : MonoBehaviour
    {
        public TextMeshProUGUI label;
        public Image image;
        
        public void SetText(string text)
        {
            label.text = text;
        }

        public void SetImage(Sprite img)
        {
            image.sprite = img;
        }

    }
}