using UnityEngine;
using UnityEngine.AI;
public class Unit : BaseEntity
{
    public bool isInSquad = false;
    public bool isWorking = false;

    private TargetBuilding targetBuildingToCapture = null;

    [SerializeField]
    UnitDataScriptable UnitData = null;
    
    Vector3 gridPosition = Vector3.zero;

    private bool leader = true;
    public Vector3 GridPosition
    {
        get { return gridPosition; }
        set { gridPosition = value; }
    }

    private float currentMoveSpeed = 1.0f;
    
    public float CurrentMoveSpeed 
    {
        get { return currentMoveSpeed; }
        set { currentMoveSpeed = value; }
    }

    private float unitSize = 1.0f;

    public float UnitSize
    {
        get { return unitSize; }
        set { unitSize = value; }
    }

    Transform BulletSlot;
    float LastActionDate = 0f;
    BaseEntity EntityTarget = null;
    TargetBuilding CaptureTarget;
    NavMeshAgent NavMeshAgent;
    public UnitDataScriptable GetUnitData { get { return UnitData; } }
    public int Cost { get { return UnitData.Cost; } }
    public int GetTypeId { get { return UnitData.TypeId; } }
    override public void Init(ETeam _team)
    {
        if (IsInitialized)
            return;

        base.Init(_team);

        HP = UnitData.MaxHP;
        maxHp = HP;
        speed = UnitData.Speed;
        dps = UnitData.DPS;
        caption = UnitData.Caption;
        unit = true;
        
        OnDeadEvent += Unit_OnDead;
        unitSize = UnitData.Size;

        if (NavMeshAgent)
            NavMeshAgent.isStopped = true;
    }
    void Unit_OnDead()
    {
        if (IsCapturing())
            StopCapture();

        if (GetUnitData.DeathFXPrefab)
        {
            GameObject fx = Instantiate(GetUnitData.DeathFXPrefab, transform);
            fx.transform.parent = null;
        }

        DefenseTask.RemovePlayerUnitFromAllList(this);
        AttackTask.RemovePlayerUnitFromAllList(this);
        Destroy(gameObject);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    #region MonoBehaviour methods
    override protected void Awake()
    {
        base.Awake();

        NavMeshAgent = GetComponent<NavMeshAgent>();
        BulletSlot = transform.Find("BulletSlot");

        // fill NavMeshAgent parameters
        NavMeshAgent.speed = GetUnitData.Speed;
        NavMeshAgent.angularSpeed = GetUnitData.AngularSpeed;
        NavMeshAgent.acceleration = GetUnitData.Acceleration;
    }
    override protected void Start()
    {
        // Needed for non factory spawned units (debug)
        if (!IsInitialized)
            Init(Team);

        base.Start();
    }
    override protected void Update()
    {
        if (targetBuildingToCapture != null)
            ComputeCapture();
        // Attack / repair task debug test $$$ to be removed for AI implementation
        if (EntityTarget != null)
        {
            if (EntityTarget.GetTeam() != GetTeam())
                ComputeAttack();
            else
                ComputeRepairing();
        }
        
        if(AIController.GetAiControllerTeam() == Team && !NavMeshAgent.isStopped)
            NavMeshAgent.isStopped = !NavMeshAgent.hasPath;
    }
    #endregion

    #region IRepairable
    override public bool NeedsRepairing()
    {
        return HP < GetUnitData.MaxHP;
    }
    override public void Repair(int amount)
    {
        HP = Mathf.Min(HP + amount, GetUnitData.MaxHP);
        base.Repair(amount);
    }
    override public void FullRepair()
    {
        Repair(GetUnitData.MaxHP);
    }
    #endregion

    #region Tasks methods : Moving, Capturing, Targeting, Attacking, Repairing ...

    // $$$ To be updated for AI implementation $$$

    // Moving Task
    public void SetTargetPos(Vector3 pos)
    {
        if (EntityTarget != null)
            EntityTarget = null;

        if (targetBuildingToCapture != null)
            StopCapture();

        if (NavMeshAgent)
        {
            NavMeshAgent.SetDestination(pos);
            NavMeshAgent.isStopped = false;
        }
    }

    // Targetting Task - attack
    public void SetAttackTarget(BaseEntity target)
    {
        if (target.GetTeam() != GetTeam())
        {
            isWorking = true;
            
            EntityTarget = target;

            if (CaptureTarget != null)
                StopCapture();
        }

    }

    // Targetting Task - capture
    public void SetCaptureTarget(TargetBuilding target)
    {
        if(target != targetBuildingToCapture)
            StopCapture();

        EntityTarget = null;
        
        targetBuildingToCapture = target;
        
        isWorking = true;
    }

    // Targetting Task - repairing
    public void SetRepairTarget(BaseEntity entity)
    {
        if (CanRepair(entity) == false)
            return;

        if (CaptureTarget != null)
            StopCapture();

        if (entity.GetTeam() == GetTeam())
            StartRepairing(entity);
    }
    public bool CanAttack(BaseEntity target)
    {
        if (target == null)
            return false;

        // distance check
        if ((target.transform.position - transform.position).sqrMagnitude > GetUnitData.AttackDistanceMax * GetUnitData.AttackDistanceMax)
            return false;

        return true;
    }

    // Attack Task
    public void StartAttacking(BaseEntity target)
    {
        EntityTarget = target;
    }
    public void ComputeAttack()
    {
        if (CanAttack(EntityTarget) == false)
        {
            if (NavMeshAgent)
            {
                NavMeshAgent.SetDestination(EntityTarget.transform.position);
                NavMeshAgent.isStopped = false;
            }
            return;
        }

        if (NavMeshAgent)
            NavMeshAgent.isStopped = true;

        transform.LookAt(EntityTarget.transform);
        // only keep Y axis
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = 0f;
        eulerRotation.z = 0f;
        transform.eulerAngles = eulerRotation;

        if ((Time.time - LastActionDate) > UnitData.AttackFrequency)
        {
            LastActionDate = Time.time;
            // visual only ?
            if (UnitData.BulletPrefab)
            {
                GameObject newBullet = Instantiate(UnitData.BulletPrefab, BulletSlot);
                newBullet.transform.parent = null;
                newBullet.GetComponent<Bullet>().ShootToward(EntityTarget.transform.position - transform.position, this);
            }
            // apply damages
            int damages = Mathf.FloorToInt(UnitData.DPS * UnitData.AttackFrequency);
            EntityTarget.AddDamage(damages);
            
            //Change unit atck
            if (!EntityTarget.IsAlive)
            {
                var unitList = GameServices.GetControllerByTeam(GameServices.GetOpponent(Team)).GetAllUnits();
                foreach (var unit in unitList)
                {
                    if (Vector3.Distance(transform.position, unit.transform.position) <= UnitData.AttackDistanceMax)
                    {
                        EntityTarget = unit;
                        return;
                    }
                }
            }
        }
    }
    public bool CanCapture(TargetBuilding target)
    {
        if (target == null)
            return false;

        // distance check
        if ((target.transform.position - transform.position).sqrMagnitude > GetUnitData.CaptureDistanceMax * GetUnitData.CaptureDistanceMax)
            return false;

        return true;
    }

    // Capture Task
    public void StartCapture(TargetBuilding target)
    {
        if (CanCapture(target) == false)
            return;

        if (NavMeshAgent)
            NavMeshAgent.isStopped = true;

        CaptureTarget = target;
        CaptureTarget.StartCapture(this);
        isWorking = true;
    }
    public void StopCapture()
    {
        
        if (CaptureTarget != null)
            CaptureTarget.StopCapture(this);

        CaptureTarget = null;
        targetBuildingToCapture = null;
        isWorking = false;
    }

    public bool IsCapturing()
    {
        return CaptureTarget != null;
    }

    private void ComputeCapture()
    {
        if (Team == targetBuildingToCapture.GetTeam())
        {
            StopCapture();
            return;
        }
        
        if (CanCapture(targetBuildingToCapture) == false)
        {
            if (NavMeshAgent)
            {
                NavMeshAgent.SetDestination(targetBuildingToCapture.transform.position);
                NavMeshAgent.isStopped = false;
            }
            return;
        }

        if (IsCapturing())
            return;

        if (targetBuildingToCapture.GetTeam() != GetTeam())
            StartCapture(targetBuildingToCapture);
    }

    // Repairing Task
    public bool CanRepair(BaseEntity target)
    {
        if (GetUnitData.CanRepair == false || target == null)
            return false;

        // distance check
        if ((target.transform.position - transform.position).sqrMagnitude > GetUnitData.RepairDistanceMax * GetUnitData.RepairDistanceMax)
            return false;

        return true;
    }
    public void StartRepairing(BaseEntity entity)
    {
        if (GetUnitData.CanRepair)
        {
            EntityTarget = entity;
        }
    }

    // $$$ TODO : add repairing visual feedback
    public void ComputeRepairing()
    {
        if (CanRepair(EntityTarget) == false)
            return;

        if (NavMeshAgent)
            NavMeshAgent.isStopped = true;

        transform.LookAt(EntityTarget.transform);
        // only keep Y axis
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = 0f;
        eulerRotation.z = 0f;
        transform.eulerAngles = eulerRotation;

        if ((Time.time - LastActionDate) > UnitData.RepairFrequency)
        {
            LastActionDate = Time.time;

            // apply reparing
            int amount = Mathf.FloorToInt(UnitData.RPS * UnitData.RepairFrequency);
            EntityTarget.Repair(amount);
        }
    }

    public bool IsStopped()
    {
        return NavMeshAgent.isStopped;
    }
    #endregion
}
