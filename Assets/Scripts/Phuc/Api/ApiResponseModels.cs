using System;
using System.Collections.Generic;

[Serializable]
public class RegisterPayload
{
    public string email, username, password;
    public RegisterPayload(string e, string u, string p) { email = e; username = u; password = p; }
}

[Serializable]
public class VerifyPayload
{
    public string email, otp;
    public VerifyPayload(string e, string o) { email = e; otp = o; }
}

[Serializable]
public class LoginPayload
{
    public string username, password;
    public LoginPayload(string u, string p) { username = u; password = p; }
}

[Serializable]
public class MessageResponse
{
    public string message;
}

[Serializable]
public class LoginResponse
{
    public string token;
    public string expires;
    public LoginUserData user;
}

[Serializable]
public class LoginUserData
{
    public int userId;
    public string username;
    public string email;
    public string role;
    public int diamonds;
    public List<CharacterData> characters;
}

[Serializable]
public class CharacterData
{
    public int id;
    public string name;
}

[Serializable]
public class PlayerStateResponse
{
    public int classId;
    public int level;
    public float positionX;
    public float positionY;
    public float positionZ;
    public int currentHealth;
    public int maxHealth;
    public int currentMana;
    public int maxMana;
    public int experience;
    public int maxArmor;
    public int maxPhysicalDamage;
    public int maxMagicDamage;
    public int maxCooldownReduction;
    public int maxMoveSpeed;
    public int maxAttackSpeed;
    public int maxAttackRange;
    public int coin;
    public string sceneName;
}

[Serializable]
public class SavePlayerStateResponse
{
    public string message;
    public int characterId;
}

[Serializable]
public class SaveNpcDialoguePayload
{
    public List<NpcDialogueEntry> entries;
    public SaveNpcDialoguePayload(List<NpcDialogueEntry> e) { entries = e; }
}

[Serializable]
public class NpcDialogueListResponse
{
    public List<NpcDialogueEntry> entries;
}

//==== Relationships ====
[Serializable]
public class Re
{
    public string npcName;
    public int value;
}

[Serializable]
public class SaveRelationshipsPayload
{
    public List<RelationEntry> entries;
    public SaveRelationshipsPayload(List<RelationEntry> e) { entries = e; }
}

[Serializable]
public class RelationshipListResponse
{
    public List<RelationEntry> entries;
}

// ==== Missions ====
[Serializable]
public class MissionEntry
{
    public int missionID;
    public string title;
    public string description;
    public string type;
    public int currentCount;
    public int targetCount;
    public string location;
    public int rewardGold;
    public int rewardExp;
    public string missionType;
    public bool isCompleted;
    public string storyID;
}

[Serializable]
public class SaveMissionsPayload
{
    public List<MissionSaveData> missions;
    public SaveMissionsPayload(List<MissionSaveData> m) { missions = m; }
}

// Response từ GET missions
[Serializable]
public class MissionResponseEntry
{
    public int missionID;
    public string title;
    public string description;
    public string type;
    public int count;
    public string location;
    public int rewardGold;
    public int rewardExp;
    public string missionType;
    public bool isCompleted;
    public string storyID;
}

[Serializable]
public class MissionListResponse
{
    public List<MissionSaveData> missions;
}

// ==== Story State ====
[Serializable]
public class StoryMovedObject
{
    public string objectID;
    public float positionX;
    public float positionY;
    public float positionZ;
}

[Serializable]
public class SaveStoryStatePayload
{
    public List<string> disabledObjects;
    public List<string> enabledObjects;
    public List<StoryMovedObject> movedObjects;
}

[Serializable]
public class StoryStateResponse
{
    public List<string> disabledObjects;
    public List<string> enabledObjects;
    public List<StoryMovedObject> movedObjects;
}

// ==== Current Story Block ====
[Serializable]
public class CurrentStoryBlockPayload
{
    public string value;
    public CurrentStoryBlockPayload(string v) { value = v; }
}
