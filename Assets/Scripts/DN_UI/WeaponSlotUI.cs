using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image Image_WeaponIcon;

    public void SetWeaponIcon(Sprite weaponSprite)
    {
        Image_WeaponIcon.sprite = weaponSprite;
    }
}
