using System.Collections.Generic;

namespace ControlCompanyAddons.Additions;

public static class HoardingBugAdditions {
    private static readonly List<HoarderBugItem> _FakeStolenItems = [
    ];

    public static void HandleAttackLogic(bool isAttacking) {
        if (!isAttacking) {
            foreach (var fakeStolenItem in (HoarderBugItem[]) [
                         .._FakeStolenItems,
                     ]) {
                _FakeStolenItems.Remove(fakeStolenItem);
                HoarderBugAI.HoarderBugItems.Remove(fakeStolenItem);
            }

            return;
        }

        foreach (var playerControllerB in RoundManager.Instance.playersManager.allPlayerScripts) {
            if (!playerControllerB.isPlayerControlled) continue;

            if (playerControllerB.isPlayerDead) continue;

            foreach (var itemSlot in playerControllerB.ItemSlots) {
                if (itemSlot is null) continue;

                var fakeStolenItem = new HoarderBugItem(itemSlot, HoarderBugItemStatus.Stolen, new(2000, 2000, 2000));

                _FakeStolenItems.Add(fakeStolenItem);

                HoarderBugAI.HoarderBugItems.Add(fakeStolenItem);
            }
        }
    }
}