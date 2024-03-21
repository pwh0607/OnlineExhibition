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

        StartCoroutine(AddCommentCoroutine(commentRef,username, comment));
        inputComment.text = "";

        IEnumerator AddCommentCoroutine(DatabaseReference commentRef, string username, string comment)
        {
            var setUsernameTask = commentRef.Child("username").SetValueAsync(username);
            yield return new WaitUntil(() => setUsernameTask.IsCompleted);

            var setCommentTask = commentRef.Child("comment").SetValueAsync(comment);
            yield return new WaitUntil(() => setCommentTask.IsCompleted);

            if (setUsernameTask.IsFaulted || setCommentTask.IsFaulted)
            {
                Debug.LogError("��� �߰� ����...");
            }
            else
            {
                Debug.Log("��� �о� ���� ��...");
                LoadComments();
            }
            yield return null;
        }
    }

    //��� ��ư Ŭ����...
    void LoadComments()
    {
        Debug.Log("������Ʈ name : " + objName);
        comments.Clear();

        //���� Ŭ����... �ش� objName �ޱ�.
        DatabaseReference commentRef = databaseRef.Child(objName).Child("comments");
        commentRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("��� �о���� ����.........");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                Debug.Log("No comments found.");
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
            Debug.Log("��� �߰� �Ϸ�");
        });
    }

     private void setCommentViewer()
    {
        int i = 0;
        Debug.Log("��� ���� : " + comments.Count);
        foreach (Comment comment in comments)
        {
            try
            {
                Debug.Log("��� ��ȣ " + (i+1));
                Debug.Log(comment.username + " : " + comment.comment);

                GameObject commentInstance = Instantiate(commentPrefab);
                RectTransform commentPos = commentInstance.GetComponent<RectTransform>();
                commentPos.SetParent(contentView.transform);
                objList.Add(commentInstance);

                Vector2 dir = new Vector2(0.5f, 1f);

                //��ġ �� ������ ����
                commentPos.anchorMin = dir;
                commentPos.anchorMax = dir;
                commentPos.pivot = dir;

                commentPos.localPosition = new Vector3(0, 0, 0);
                commentPos.anchoredPosition = new Vector2(0, -80f - (290 * i));
                commentPos.localScale = new Vector3(4, 4, 1);

                //��� ���� ����
                commentInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = comment.username;
                commentInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = comment.comment;

                i++;
            }catch(Exception e)
            {
                Debug.LogError("Error :" + e);
            }
        }
    }
    
    public void OnclickCommentBtn()
    {
        LoadComments();
    }

    public void setActionCommentBtn(bool act)
    {
        commentBtn.gameObject.SetActive(act);
        Debug.Log("�׼ǽ���!");
    }

    public void setCommentPart()
    {
        commentPart.SetActive(!commentPart.active);
    }

    public void updateCommentList()
    {
        //������ ��� ������Ʈ�� ����;
        Debug.Log("��ۼ� : "+comments.Count);
        foreach(GameObject obj in objList){
            Destroy(obj);
        }
        setCommentViewer();
    }
}
struct Comment
{
    public string commentId;
    public string username;
    public string comment;

    public Comment(string commentId, string username, string comment)
    {
        this.commentId = commentId;
        this.username = username;
        this.comment = comment;
    }
}