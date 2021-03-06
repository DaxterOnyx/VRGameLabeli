﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerPCSpawn))]
[RequireComponent(typeof(PlayerPCMotor))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerPC : MonoBehaviour {

    /// Components  
     
    protected PlayerPCSpawn playerPCSpawn;
    protected PlayerPCMotor playerPCMotor;
    protected Rigidbody rb;


    /// Attributs

    [SerializeField] protected float currentMana = 0f;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaRegen = 5f;
    [SerializeField] private float maxManaClamp = 150f;
    [Range(5, 25)] [SerializeField] private int distanceMinSpawn = 15;
    public GameObject[] allEnemyTab;
    public int enemyIndex;

    /// Instanciations

    [SerializeField] protected GameObject UIPlayerPCPrefabs;
    private GameObject UIPlayerPCInstance;

    private void Awake()
    {
        playerPCMotor = GetComponent<PlayerPCMotor>();
        playerPCSpawn = GetComponent<PlayerPCSpawn>();
        rb = GetComponent<Rigidbody>();
        UIPlayerPCInstance = Instantiate(UIPlayerPCPrefabs);
        UIPlayerPCInstance.name = UIPlayerPCPrefabs.name;
    }

    private void Start()
    {
        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        Spawn();
    }

    private void Spawn()
    {
        if (PlayerPCController.Click0_Down)
        {
            Ray ray_ = new Ray(this.transform.position, playerPCMotor.eyes.TransformDirection(Vector3.forward));
            RaycastHit hit_ = new RaycastHit();

            if (Physics.Raycast(ray_, out hit_, Mathf.Infinity))
            {
                if (hit_.collider.gameObject.tag == "Terrain")
                {
                    playerPCSpawn.Spawn_One_Spawner(hit_);
                }
            }
        }
    }


    #region Ancien script
    /*
        // scripts du player
    private PlayerPCSpawn playerPCSpawn;
    private PlayerPCMotor playerPCMotor;
        // scripts du UI
    private BonusMenu bonusMenuScript;
    private ManaBarScript manaBarScript;
    private EnemyMenu enemyMenuScript;

    private Camera camScript;

    [SerializeField] private GameObject UIPlayerPCPrefabs;
    [SerializeField] private GameObject cam;
    private GameObject UIPlayerPCInstance;
    private GameObject bonusMenuGo;
    private Transform target;
    [SerializeField] GameObject touchHelp;

    private float currentMana;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaRegen = 5f;
    [Range(0f, 1000f)] [SerializeField] private float maxManaClamp = 150f;
    [Range(5f, 25f)] [SerializeField] private float distanceMinSpawn = 15f;

    Image imageCursor;

	private void Start ()
    {
        currentMana = 0f;

        playerPCSpawn = GetComponent<PlayerPCSpawn>();
        playerPCController = GetComponent<PlayerPCController>();
        playerPCMotor = GetComponent<PlayerPCMotor>();

        UIPlayerPCInstance = Instantiate(UIPlayerPCPrefabs);
        UIPlayerPCInstance.name = UIPlayerPCPrefabs.name;

        bonusMenuScript = UIPlayerPCInstance.GetComponent<BonusMenu>();
        enemyMenuScript = UIPlayerPCInstance.GetComponent<EnemyMenu>();
        manaBarScript = UIPlayerPCInstance.GetComponent<ManaBarScript>();

        playerPCSpawn.SetUI(UIPlayerPCInstance);
        enemyMenuScript.SetAllEnemyTab(playerPCSpawn.allEnemyTab);
        bonusMenuScript.SetAllEnemyTab(playerPCSpawn.allEnemyTab);
        manaBarScript.SetMana(maxMana, currentMana);
        manaBarScript.SetManaRegen(manaRegen);

        bonusMenuGo = GameObject.FindGameObjectWithTag("BonusMenu");
        imageCursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Image>();
        bonusMenuGo.SetActive(false);
    }
	
	private void Update ()
    {
        if (GameManager.Instance.matchIsProgress)
        {
            touchHelp.SetActive(false);
            imageCursor.gameObject.SetActive(true);
            maxMana = Mathf.Clamp(maxMana + (Time.deltaTime * 0.625f), 0f, maxManaClamp);

            if (currentMana < maxMana)
            {
                RegenMana();
            }

            if(target == null)
            {
                GameObject JoueurVR = GameObject.FindGameObjectWithTag("Player");
                if(JoueurVR != null)
                {
                    target = JoueurVR.transform;
                }
                else
                {
                    return;
                }
            }

            Spawn();
        }
        else
        {
            imageCursor.gameObject.SetActive(false);
        }
        
        Controller();
    }

    private void RegenMana()
    {
        currentMana = currentMana + manaRegen * Time.deltaTime;

        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        manaRegen = manaBarScript.SetMana(maxMana, currentMana);
    }

    private void Spawn()
    {
        Ray ray = new Ray(this.transform.position, playerPCMotor.Eye.transform.TransformDirection(Vector3.forward));
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "Terrain")
            {
                float distanceVRCursor = Mathf.Abs(Vector3.Distance(hit.point, target.transform.position));

                if (distanceVRCursor >= distanceMinSpawn && playerPCSpawn.CanSpawn())
                {
                    float lostMana = bonusMenuScript.UpdateLostMana();

                    if(currentMana >= lostMana)
                    {
                        imageCursor.color = Color.HSVToRGB(0f, 0f, 0f);   // Noir
                    }
                    else
                    {
                        imageCursor.color = Color.HSVToRGB(0f, 100f, 85f);   // Rouge
                    }

                    if (playerPCController.ClickDown)
                    {
                        if (currentMana >= lostMana)
                        {                    
                            StartCoroutine(playerPCSpawn.Spawn(hit, playerPCSpawn.GetEnemySpawn(), playerPCSpawn.GetCount()));
                            currentMana -= lostMana;
                        }
                    }
                }
                else
                {
                    imageCursor.color = Color.HSVToRGB(0f, 100f, 85f);   // Rouge
                }
            }
        }  
    }

    private void Controller()
    {
        // Si le joueur appuie sur la touche "BonusMenu"
        if (playerPCController.BonusMenu)
        {
            bonusMenuGo.SetActive(!bonusMenuGo.activeSelf);
            bonusMenuScript.UpdateText();
        }

        // Si le menu des bonus est désactivé
        if(bonusMenuGo.activeSelf == false)
        {
            if (playerPCController.E || playerPCController.ScrollWheel < 0f)
            {
                playerPCSpawn.ChangeEnemy(enemyMenuScript.IconRight());
            }
            else if (playerPCController.A || playerPCController.ScrollWheel > 0f)
            {
                playerPCSpawn.ChangeEnemy(enemyMenuScript.IconLeft());
            }
        }
        else // Si le menu des bonus est activé
        {
            if (playerPCController.ScrollWheel < 0f)
            {
                bonusMenuScript.IconUp();
            }
            else if (playerPCController.ScrollWheel > 0f)
            {
                bonusMenuScript.IconDown();
            }

            if (playerPCController.E)
            {
                bonusMenuScript.UpgradeBonus();
            }
            else if (playerPCController.A)
            {
                bonusMenuScript.DowngradeBonus();
            }
        } 
    }

    */
    #endregion
}
