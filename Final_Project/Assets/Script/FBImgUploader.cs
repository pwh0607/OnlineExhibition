using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;

public class FBImgUploader : MonoBehaviour
{
    //FB 참조
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string roomNumber;          //FB storage 폴더 접근용.
    public GameObject cube;     //이미지

}