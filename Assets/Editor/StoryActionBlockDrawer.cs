using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StoryActionBlock))]
public class StoryActionBlockDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 1; // always show 'action'

        StoryAction action = (StoryAction)property.FindPropertyRelative("action").enumValueIndex;

        switch (action)
        {
            case StoryAction.Spawn:
                lines += 3; // prefabs, count, position
                break;
            case StoryAction.Mission:
                lines += 1; // missionRaw
                break;
            case StoryAction.MoveObject:
                lines += 2; // position
                break;
            case StoryAction.EnableObject:
                lines += 1; // targetID
                break;
            case StoryAction.DisableObject:
                lines += 1; // targetID
                break;
            case StoryAction.NPCRelationShip:
                lines += 2; // relationShipEff
                break;
            case StoryAction.NPCDialogue:
                lines += 2; // dialogID, 
                break;
            case StoryAction.StoryIDSetup:
                lines += 1; // storyID
                break;
            case StoryAction.StoryIDSetupWithOutSave:
                lines += 1; // storyID
                break;
            case StoryAction.Wait:
                lines += 2; // waitTime
                break;

        }

        return EditorGUIUtility.singleLineHeight * lines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        var actionProp = property.FindPropertyRelative("action");
        EditorGUI.PropertyField(line, actionProp);
        line.y += EditorGUIUtility.singleLineHeight;

        StoryAction action = (StoryAction)actionProp.enumValueIndex;

        void Draw(string propName)
        {
            var prop = property.FindPropertyRelative(propName);
            EditorGUI.PropertyField(line, prop);
            line.y += EditorGUIUtility.singleLineHeight;
        }

        switch (action)
        {
            case StoryAction.Spawn:
                Draw("prefabs");
                Draw("count");
                Draw("position");
                break;
            case StoryAction.Mission:
                Draw("missionRaw");
                break;
            case StoryAction.MoveObject:
                Draw("targetID");
                Draw("position");
                break;
            case StoryAction.EnableObject:
                Draw("targetID");
                break;
            case StoryAction.DisableObject:
                Draw("targetID");
                break;
            case StoryAction.NPCRelationShip:
                Draw("targetID");
                Draw("relationShipEff");
                break;
            case StoryAction.NPCDialogue:
                Draw("targetID");
                Draw("dialogID");
                break;
            case StoryAction.StoryIDSetup:
                Draw("storyID");
                break;
            case StoryAction.StoryIDSetupWithOutSave:
                Draw("storyID");
                break;
            case StoryAction.Wait:
                Draw("waitTime");
                Draw("Notification");
                break;

        }
    }
}
