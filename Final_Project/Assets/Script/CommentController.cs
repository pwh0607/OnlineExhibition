using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;

using TMPro;
using Photon.Pun;

public class CommentController : MonoBehaviourPun
{
    List<Comment> comments = new List<Comment>();
    List<GameObject> objList = new List<GameObject>();

    FBManager fbManager;
    string objName;
    DatabaseReference databaseRef;

    public TMP_InputField inputComment;
    public GameObject contentView;
    public GameObject commentPart;

    public GameObject commentPrefab;

    public Button commentBtn;

    void Start()
    {
        objName = GetComponent<FrameController>().getObjName();
        fbManager = FBManager.instance;
        commentPart.SetActive(false);
        commentBtn.gameObject.SetActive(false);

        if (fbManager != null)
        {
            databaseRef = fbManager.GetDatabaseReference();
        }
    }

    public void addComment()
    {
        string username = PhotonNetwork.NickName;   
        string comment = inputComment.text;

        DatabaseReference commentRef = databaseRef.Child(objName).Child("comments").Push();

        StartCoroutine(AddCommentWithCoroutine(commentRef,username, comment));
        inputComment.text = "";

        IEnumerator AddCommentWithCoroutine(DatabaseReference commentRef, string username, string comment)
        {
            var setUsernameTask = commentRef.Child("username").SetValueAsync(username);
            yield return new WaitUntil(() => setUsernameTask.IsCompleted);

            var setCommentTask = commentRef.Child("comment").SetValueAsync(comment);
            yield return new WaitUntil(() => setCommentTask.IsCompleted);

            if (setUsernameTask.IsFaulted || setCommentTask.IsFaulted)
            {
            }
            else
            {
                LoadComments();
            }
            yield return null;
        }
    }

    //댓글 버튼 클릭시...
    public void LoadComments()
    {
        comments.Clear();

        DatabaseReference commentRef = databaseRef.Child(objName).Child("comments");
        commentRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                return;
            }
            foreach (var commentSnapshot in snapshot.Children)
            {
                IDictionary<string, object> commentData = (IDictionary<string, object>)commentSnapshot.Value;
                string usernameTmp = (string)commentData["username"];
                string commentTmp = (string)commentData["comment"];
                string commentUID = commentSnapshot.Key;
                
                Comment tmp = new Comment(commentUID, usernameTmp, commentTmp);
                comments.Add(tmp);
            }
            updateCommentList();
        });
    }

     private void setCommentViewer()
    {
        int i = 0;
        foreach (Comment comment in comments)
        {
            GameObject commentInstance = Instantiate(commentPrefab);
            RectTransform commentPos = commentInstance.GetComponent<RectTransform>();
            commentPos.SetParent(contentView.transform);
            commentInstance.GetComponent<CommentContentController>().setParentObj(gameObject, comment.key, comment.comment);
                
            objList.Add(commentInstance);

            Vector2 dir = new Vector2(0.5f, 1f);

            commentPos.anchorMin = dir;
            commentPos.anchorMax = dir;
            commentPos.pivot = dir;

            commentPos.localPosition = new Vector3(0, 0, 0);
            commentPos.anchoredPosition = new Vector2(0, -80f - (290 * i));
            commentPos.localScale = new Vector3(4, 4, 1);

            commentInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = comment.username;
            commentInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = comment.comment;

            i++;
        }
    }
    
    public void OnclickCommentBtn()
    {
        LoadComments();
    }

    public void setActionCommentBtn(bool act)
    {
        commentBtn.gameObject.SetActive(act);
    }

    public void setCommentPart()
    {
        commentPart.SetActive(!commentPart.active);
    }

    public void updateCommentList()
    {
        foreach(GameObject obj in objList){
            Destroy(obj);
        }
        setCommentViewer();
    }

    public void removeComment(GameObject comment)
    {
        objList.Remove(comment);
    }
}
struct Comment
{
    public string key;
    public string username;
    public string comment;

    public Comment(string key, string username, string comment)
    {
        this.key = key;
        this.username = username;
        this.comment = comment;
    }
}