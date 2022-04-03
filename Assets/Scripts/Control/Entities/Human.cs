using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public class Human : Entity
{
	public Weapon Weapon;

	private HumanState _currentState;

	protected override void Awake()
	{
		base.Awake();

		_currentState = HumanState.Idle;
		transform.localScale = Vector3.one;

		Weapon.OnHit += this.HitEntity;
	}

	protected override void Update()
	{
		base.Update();

		this.Move(Vector3.zero);
		this.Attack();
	}
	
	protected override void Move(Vector3 delta)
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			base.Move(Vector3.up * HumanPreset.MoveSpeed * Time.timeScale);
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			base.Move(Vector3.down * HumanPreset.MoveSpeed * Time.timeScale);
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			base.Move(Vector3.left * HumanPreset.MoveSpeed * Time.timeScale);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			base.Move(Vector3.right * HumanPreset.MoveSpeed * Time.timeScale);
		}
	}

	private void Attack()
	{
		if (Weapon.IsHitting())
		{
			List<Entity> hitEntities = _collisionsToolkit.GetHittingEntities(transform, HitboxCollider);

			foreach (Entity hitEntity in hitEntities)
			{
				this.HitEntity(hitEntity);
			}
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Weapon.Attack();
		}
	}

	private void HitEntity(Entity entityToHit)
	{
		if (entityToHit.CanTakeHit() && entityToHit != this)
		{
			Vector3 hitDirection = (entityToHit.transform.position - transform.position).normalized;
			entityToHit.TakeHit(HumanPreset.Power, -hitDirection);
		}
	}

	protected override bool IsAlive()
	{
		return _currentState != HumanState.Dead;
	}

	protected override void Die()
	{
		this.PublishDeath();
	}

	private HumanPreset HumanPreset
	{
		get
		{
			return (HumanPreset)Preset;
		}
	}
}