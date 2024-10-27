using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using TMPro;

public class CommentContentController : MonoBehaviour
{
    string key;
    string content;
    GameObject obj;

    public GameObject contentMenu;

    public TMP_InputField modInputField;
    public GameObject frame;

    FBManager fbManager;
    DatabaseReference databaseRef;


    public void OnclickMenuBar()
    {
        contentMenu.SetActive(!contentMenu.active);
    }

    public void OnClickDeleteBtn()
    {
        if (fbManager != null)
        {
            DatabaseReference commentRef = databaseRef.Child(obj.GetComponent<FrameController>().getObjName()).Child("comments").Child(key);

            commentRef.RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                frame.GetComponent<CommentController>().removeComment(gameObject);
                frame.GetComponent<CommentController>().updateCommentList();
            });
        }

        frame.GetComponent<CommentController>().LoadComments();
    }
    public void OnClickModifyBtn()
    {
        modInputField.gameObject.SetActive(true);
        modInputField.text = content;
    }

    public void completeModify()
    {
        DatabaseReference commentRef = databaseRef.Child(obj.GetComponent<FrameController>().getObjName()).Child("comments").Child(key);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "comment", modInputField.text }
        };

        commentRef.UpdateChildrenAsync(updates);
        modInputField.gameObject.SetActive(false);
        frame.GetComponent<CommentController>().LoadComments();
        OnclickMenuBar();
    }
    public void setParentObj(GameObject obj, string key, string content)
    {
        this.obj = obj;
        this.key = key;
        this.content = content;
    }
    void FBinit()
    {
        fbManager = FBManager.instance;
        databaseRef = fbManager.GetDatabaseReference();
    }

    void Start()
    {
        FBinit();
    }
}
