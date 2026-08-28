using BaldiPlusRandomZone.EndlessSupport;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiPlusRandomZone.PitStop
{
    public class UpgradableGreenLocker : MonoBehaviour
    {
        public void Initialize(StorageLocker locker, bool hideUnavalibale)
        {
            this.locker = locker;
            pickups = (Pickup[])locker.ReflectionGetVariable("pickup");

            for (int i = 0; i < pickups.Length; i++)
            {
                if (!Singleton<EndlessZoneManager>.Instance.upgradedLockerSlots[i])
                {
                    if (hideUnavalibale)
                    {
                        pickups[i].Hide(true);
                        this.hideUnavalibale[i] = true;
                        continue;
                    }


                    pickups[i].itemSprite.sprite = Plugin.assetMan.Get<Sprite>("UnvaliableSlot");
                    pickups[i].OnItemPurchased += OnBuySlot;
                    pickups[i].price = 500;
                    pickups[i].free = false;
                }

                pickups[i].showDescription = true;
                this.hideUnavalibale[i] = false;
            }
        }

        private void Update()
        {
            for (int i = 0; i < pickups.Length; i++)
            {
                if (pickups[i] != null && !Singleton<EndlessZoneManager>.Instance.upgradedLockerSlots[i])
                {
                    if (!hideUnavalibale[i])
                        pickups[i].itemSprite.sprite = Plugin.assetMan.Get<Sprite>("UnvaliableSlot");
                    else
                        pickups[i].Hide(true);
                }
            }
        }

        public void OnBuySlot(Pickup pickup, int player)
        {
            pickup.free = true;
            pickup.itemSprite.sprite = pickup.item.itemSpriteLarge;

            for (int i = 0; i < pickups.Length; i++)
            {
                if (pickup == pickups[i])
                    Singleton<EndlessZoneManager>.Instance.upgradedLockerSlots[i] = true;
            }
        }

        public StorageLocker locker;
        public Pickup[] pickups = new Pickup[3];
        public bool[] hideUnavalibale = new bool[3];
    }
}
