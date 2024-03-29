namespace ControlCompanyAddons.Additions;

public static class MouthDogAdditions {
    public static void HandleScreamOrLunge(MouthDogAI mouthDogAI, bool hasScreamed) {
        mouthDogAI.SwitchToBehaviourClientRpc(hasScreamed ? 2 : 3);
    }
    
    public static void HandleAngerState(MouthDogAI mouthDogAI, bool wasAngered) {
        mouthDogAI.SwitchToBehaviourClientRpc(wasAngered ? 1 : 0);
    }
}