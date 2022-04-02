using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class Entity : MonoBehaviour
{
	private const float HIT_COOLDOWN = 0.5f;
	private const float GROW_SPEED = 0.05f;

	public EntityType Type;
	public EntityPreset Preset;

	public float Vitality;

	public CircleCollider2D HitboxCollider;

	protected Guid _id;

	protected CollisionsToolkit _collisionsToolkit;

	private float _lastHitTime;

	public event Action<EntityType, Vector3> OnBirth;
	public event Action<Entity> OnDeath;

	protected virtual void Awake()
	{
		_id = Guid.NewGuid();

		_collisionsToolkit = new CollisionsToolkit();

		_lastHitTime = 0;
		
		transform.localScale = Vector3.zero;
	}

	protected virtual void Start()
	{
		
	}

	protected virtual void Update()
	{

	}

	protected virtual void FixedUpdate()
	{
		Vitality = Mathf.Clamp(Vitality + Preset.VitalitySpeed, 0, Preset.MaxVitality);

		if (Vitality <= 0)
		{
			this.Die();
		}
	}

	protected void GrowIfRequired()
	{
		if (transform.localScale.x < 1)
		{
			float scaleFactor = Math.Min(transform.localScale.x + GROW_SPEED, 1);
			transform.localScale = Vector3.one * scaleFactor;
		}
	}

	public virtual bool TakeHit(float damage, Vector3 hitDirection)
	{
		_lastHitTime = Time.fixedTime;

		Vitality = Math.Max(Vitality - damage, 0);
		bool isDead = Vitality <= 0;

		if (isDead)
		{
			this.Die();
		}

		return isDead;
	}

	public bool CanTakeHit()
	{
		return Time.fixedTime >= _lastHitTime + HIT_COOLDOWN;
	}

	public abstract void Die();

	public void PublishBirth(Vector3 position)
	{
		OnBirth?.Invoke(Type, position);
	}

	public void PublishDeath()
	{
		OnDeath?.Invoke(this);
	}

	public Guid Id { get => _id; set => _id = value; }
}
