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
                Debug.LogError("댓글 추가 실패...");
            }
            else
            {
                Debug.Log("댓글 읽어 오는 중...");
                LoadComments();
            }
            yield return null;
        }
    }

    //댓글 버튼 클릭시...
    void LoadComments()
    {
        Debug.Log("오브젝트 name : " + objName);
        comments.Clear();

        //액자 클릭시... 해당 objName 받기.
        DatabaseReference commentRef = databaseRef.Child(objName).Child("comments");
        commentRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("댓글 읽어오기 실패.........");
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
            Debug.Log("댓글 추가 완료");
        });
    }

     private void setCommentViewer()
    {
        int i = 0;
        Debug.Log("댓글 개수 : " + comments.Count);
        foreach (Comment comment in comments)
        {
            try
            {
                Debug.Log("댓글 번호 " + (i+1));
                Debug.Log(comment.username + " : " + comment.comment);

                GameObject commentInstance = Instantiate(commentPrefab);
                RectTransform commentPos = commentInstance.GetComponent<RectTransform>();
                commentPos.SetParent(contentView.transform);
                objList.Add(commentInstance);

                Vector2 dir = new Vector2(0.5f, 1f);

                //위치 및 스케일 조정
                commentPos.anchorMin = dir;
                commentPos.anchorMax = dir;
                commentPos.pivot = dir;

                commentPos.localPosition = new Vector3(0, 0, 0);
                commentPos.anchoredPosition = new Vector2(0, -80f - (290 * i));
                commentPos.localScale = new Vector3(4, 4, 1);

                //댓글 내용 세팅
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
        Debug.Log("액션실행!");
    }

    public void setCommentPart()
    {
        commentPart.SetActive(!commentPart.active);
    }

    public void updateCommentList()
    {
        //기존의 댓글 오브젝트들 삭제;
        Debug.Log("댓글수 : "+comments.Count);
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