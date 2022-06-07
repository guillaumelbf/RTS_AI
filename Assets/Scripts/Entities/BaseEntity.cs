using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseEntity : MonoBehaviour, ISelectable, IDamageable, IRepairable
{
    [SerializeField]
    protected ETeam Team;

    protected int HP = 0;
    
    protected int maxHp = 0;
    protected string caption = "";
    protected float speed = 0;
    protected float dps = 0;

    protected Action OnHpUpdated;
    protected GameObject SelectedSprite = null;
    protected Text HPText = null;
    protected Text SpeedText = null;
    protected Text CaptionText = null;
    protected Text DpsText = null;
    protected bool unit = false;
    
    
    protected bool IsInitialized = false;

    public Action OnDeadEvent;
    public bool IsSelected { get; protected set; }
    public bool IsAlive { get; protected set; }
    virtual public void Init(ETeam _team)
    {
        if (IsInitialized)
            return;

        Team = _team;

        IsInitialized = true;
    }
    public Color GetColor()
    {
        return GameServices.GetTeamColor(GetTeam());
    }
    void UpdateHpUI()
    {
        if (HPText != null)
            HPText.text = "HP : " + HP.ToString() + "/" + maxHp.ToString();
    }

    void ShowUI()
    {
        
        if(HPText != null)
            if (IsSelected)
            {
                if (!GameServices.UiStatus)
                {
                    GameServices.UiStatus = true;
                    HPText.gameObject.SetActive(true);
                    CaptionText.gameObject.SetActive(true);
                    if (unit)
                    {
                        SpeedText.gameObject.SetActive(true);
                        DpsText.gameObject.SetActive(true);
                    }
                    Debug.Log("affichage UI");
                }
                
            }
            else
            {
                if (GameServices.UiStatus)
                {
                    GameServices.UiStatus = false;
                    CaptionText.gameObject.SetActive(false);
                    if (unit)
                    {
                        SpeedText.gameObject.SetActive(false);
                        DpsText.gameObject.SetActive(false);
                    }
                    HPText.gameObject.SetActive(false);
                    Debug.Log("hidden UI");
                }
                
            }
            
    }

    void SetUI()
    {
        if (CaptionText != null)
            CaptionText.text = caption;

        if (SpeedText != null)
            SpeedText.text = "Speed : " + speed.ToString();

        if (DpsText != null)
            DpsText.text = "Dps : " + dps.ToString();

    }

    #region ISelectable
    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        SelectedSprite?.SetActive(IsSelected);
        
        ShowUI();
    }
    public ETeam GetTeam()
    {
        return Team;
    }
    #endregion

    #region IDamageable
    public void AddDamage(int damageAmount)
    {
        if (IsAlive == false)
            return;

        HP -= damageAmount;

        OnHpUpdated?.Invoke();

        if (HP <= 0)
        {
            IsAlive = false;
            OnDeadEvent?.Invoke();
            Debug.Log("Entity " + gameObject.name + " died");
        }
    }
    public void Destroy()
    {
        AddDamage(HP);
    }
    #endregion

    #region IRepairable
    virtual public bool NeedsRepairing()
    {
        return true;
    }
    virtual public void Repair(int amount)
    {
        OnHpUpdated?.Invoke();
    }
    virtual public void FullRepair()
    {
    }
    #endregion

    #region MonoBehaviour methods
    virtual protected void Awake()
    {
        IsAlive = true;

        SelectedSprite = transform.Find("SelectedSprite")?.gameObject;
        SelectedSprite?.SetActive(false);
/*
        Transform hpTransform = transform.Find("Canvas/HPText");
        if (hpTransform)
            HPText = hpTransform.GetComponent<Text>();
*/
        Transform hpTransform = transform.Find("UnitCanvas/box/UnitHp");
        if (hpTransform)
            HPText = hpTransform.GetComponent<Text>();

        Transform speedTransform = transform.Find("UnitCanvas/box/UnitSpeed");
        if (speedTransform)
            SpeedText = speedTransform.GetComponent<Text>();

        Transform dpsTransform = transform.Find("UnitCanvas/box/UnitDps");
        if (dpsTransform)
            DpsText = dpsTransform.GetComponent<Text>();

        Transform captionTransform = transform.Find("UnitCanvas/box/UnitCaption");
        if (captionTransform)
            CaptionText = captionTransform.GetComponent<Text>();

        

        OnHpUpdated += UpdateHpUI;
    }
    virtual protected void Start()
    {
        UpdateHpUI();
        SetUI();
    }
    virtual protected void Update()
    {
    }
    #endregion
}
