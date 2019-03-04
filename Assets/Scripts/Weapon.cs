using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private SpriteRenderer bulletRend;
    public GameObject bulletPrefab; //prefab dei proiettili del player
    public GameObject bullet, straightBullet, diagBullet1, diagBullet2, rearBullet; //conterranno i riferimenti ai vari tipi di proiettili istanziati
    private BulletController bulletStats; //usato per impostare i proiettili sparati
    private Collider2D[] hitObjects; //contiene tutti i collider colpiti dall'attacco meele
    public bool isAttacking = false, isShooting = false; //indicano se il player sta attaccando
    private ItemStats actualWeapon; //contiene le informazioni dell'arma equipaggiata
    private Room actualRoom;
    private Quaternion rot, rot1, rot2, rot3; //rotazioni dei proiettili
    private Vector3 dir, dir1, dir2, dir3; //direzioni dei proiettili
    private PlayerHealth ph;
    private Animator animator;

	//offset utilizzati per calcolare l'area d'attacco meele per le quattro direzioni
    public Vector3 upDistance = new Vector3(0, 0.75f, 0), downDistance = new Vector3(0, -0.75f, 0), leftDistance = new Vector3(-0.75f, 0, 0), rightDistance = new Vector3(0.75f, 0, 0);

    private Vector3 attackStart; //effettiva posizione di partenza per l'attacco meele
	public float attackDuration = 0.4f; //durata dell'attacco meele
	private float shootTimer = 0;  //timer dell'attacco ranged
	private float attackTimer = 0; //timer dell'attacco meele
	public float radius = 0.75f; //range d'attacco meele

	private AudioSource source;

    public AudioClip swing, unsheathe, cut; //suoni relativi all'arma meele
    public AudioClip shoot, pickRanged; //suoni relativi all'arma ranged

	private bool hitEnemy = false;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        ph = GetComponentInParent<PlayerHealth>();
		source = GetComponentInParent<AudioSource>();
		actualWeapon = new ItemStats(); //inizializzo l'arma
	}

    void Update()
    {
        if (!GameManager.manager.gamePause)
        {
			//se un'arma è equipaggiata
            if (actualWeapon != null)
            {
				//se l'arma è meele attacco nella direzione del tasto premuto
                if (actualWeapon.weaponType == ItemStats.WeaponType.meele)
                {
                    if (Input.GetKeyDown("up"))
                    {
						//se lo status di attacco invertito è attivo attacco nella direzione opposta
                        if (!ph.flipAtt)
                            Attack("up", transform.position);
                        else
                            Attack("down", transform.position);
                    }
                    else if (Input.GetKeyDown("down"))
                    {
                        if (!ph.flipAtt)
                            Attack("down", transform.position);
                        else
                            Attack("up", transform.position);
                    }
                    else if (Input.GetKeyDown("left"))
                    {
                        if (!ph.flipAtt)
                            Attack("left", transform.position);
                        else
                            Attack("right", transform.position);
                    }
                    else if (Input.GetKeyDown("right"))
                    {
                        if (!ph.flipAtt)
                            Attack("right", transform.position);
                        else
                            Attack("left", transform.position);
                    }
                }
                //idem come sopra: la sola differenza è che con le armi ranged posso tenere premuto il tasto per sparare in modo continuo
                else
                {
                    if (Input.GetKey("up"))
                    {
                        if (!ph.flipAtt)
                            Attack("up", transform.position);
                        else
                            Attack("down", transform.position);
                    }
                    else if (Input.GetKey("down"))
                    {
                        if (!ph.flipAtt)
                            Attack("down", transform.position);
                        else
                            Attack("up", transform.position);
                    }
                    else if (Input.GetKey("left"))
                    {
                        if (!ph.flipAtt)
                            Attack("left", transform.position);
                        else
                            Attack("right", transform.position);
                    }
                    else if (Input.GetKey("right"))
                    {
                        if (!ph.flipAtt)
                            Attack("right", transform.position);
                        else
                            Attack("left", transform.position);
                    }
                }

            }

			//aggiorno i counter degli attacchi per implementare il giusto delay tra un attacco e l'altro
            attackTimer += Time.deltaTime;
            shootTimer += Time.deltaTime;

			//se è passato il tempo relativo al fire rate dell'arma, segnalo che posso nuovamente attaccare
            if (actualWeapon.weaponType == ItemStats.WeaponType.ranged && shootTimer >= actualWeapon.fireRate)
                isShooting = false;
            if (attackTimer >= attackDuration)
                isAttacking = false;
        }
    }

	//Esegue un attacco con l'arma equipaggiata nella direzione data
    public void Attack(string direction, Vector3 playerPos)
    {
        switch (actualWeapon.weaponType)
        {
            case ItemStats.WeaponType.meele:
				//se è passato il tempo relativo al fire rate dell'arma posso attaccare di nuovo
				if (attackTimer >= attackDuration)
				{
					isAttacking = true; //segnalo che sto attaccando
					attackTimer = 0; //riazzero il timer
                    Slash(direction, playerPos); //eseguo l'attacco
                }   
                break;
            case ItemStats.WeaponType.ranged:
                if (shootTimer >= actualWeapon.fireRate)
                {
                    isShooting = true; //segnalo che sto attaccando
					shootTimer = 0; //riazzero il timer
					//se sono attivi status che alterano la velocità modifico anche la velocità dei proiettili
					if (ph.faster)
                        Shoot(direction, playerPos, actualWeapon.shotSpeed + 3);
                    else if (ph.slower)
                        Shoot(direction, playerPos, actualWeapon.shotSpeed -2);
                    else
                        Shoot(direction, playerPos, actualWeapon.shotSpeed);
                }
                break;
        }
    }

    //Esegue l'attacco relativo al tipo di arma da fuoco equipaggiata
    public void Shoot(string direction, Vector3 playerPos, float shotSpeed)
    {
        source.PlayOneShot(shoot);

		//controllo il tipo di proiettile dell'arma equipaggiata
        switch (actualWeapon.fireType)
        {
			//fuoco normale e splitShot hanno comportamento uguale, ma il secondo, quando esplode, si divide in quattro nuovi proiettili
            case ItemStats.FireType.single:
            case ItemStats.FireType.splitShot:
				//controllo la direzione di fuoco
                if (direction.Equals("left"))
                {
					//calcolo la rotazione del proiettile da istanziare
                    rot = Quaternion.AngleAxis(90, Vector3.forward);
                    dir = rot * Vector3.up;
					//istanzio il proiettile nella giusta posizione rispetto al player
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot) as GameObject;
                }
                else if (direction.Equals("right"))
                {
                    rot = Quaternion.AngleAxis(-90, Vector3.forward);
                    dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot) as GameObject;
                }
                else if (direction.Equals("up"))
                {
                    rot = Quaternion.AngleAxis(0, Vector3.forward);
                    dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), Quaternion.identity) as GameObject;
                }
                else if (direction.Equals("down"))
                {
                    rot = Quaternion.AngleAxis(180, Vector3.forward);
                    dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), Quaternion.identity) as GameObject;
                }

				//imposto le caratteristiche del proiettile
                bulletStats = bullet.GetComponent<BulletController>();
                bulletStats.SetStats(actualWeapon, transform.position);
                bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * shotSpeed;

				//aggiungo il proiettile alla lista degli spriteRenderer da ordinare
                actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Add(bullet);
                break;

			//in caso di fuoco multiplo istanzio tre proiettili che andranno in tre direzioni diverse
            case ItemStats.FireType.multiple:
                if (direction.Equals("left"))
                {
                    rot1 = Quaternion.AngleAxis(90, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(70, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(110, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot3) as GameObject;
                }
                else if (direction.Equals("right"))
                {
                    rot1 = Quaternion.AngleAxis(-90, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(-70, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(-110, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot3) as GameObject;
                }
                else if (direction.Equals("up"))
                {
                    rot1 = Quaternion.AngleAxis(0, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(-20, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(20, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot3) as GameObject;
                }
                else if (direction.Equals("down"))
                {
                    rot1 = Quaternion.AngleAxis(180, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(160, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(-160, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot3) as GameObject;
                }

                straightBullet.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * shotSpeed;

                diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * shotSpeed;

                diagBullet2.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                diagBullet2.GetComponent<Rigidbody2D>().velocity = dir3.normalized * shotSpeed;

                actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Add(straightBullet);
                actualRoom.toSort.Add(diagBullet1);
                actualRoom.toSort.Add(diagBullet2);
                break;

			//in caso di fuoco bidirezionale sparo anche nella direzione opposta a quella scelta
            case ItemStats.FireType.bidirectional:
                if (direction.Equals("left") || direction.Equals("right"))
                {
                    rot1 = Quaternion.AngleAxis(90, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(-90, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot1) as GameObject;
                    rearBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot2) as GameObject;
                }
                else if (direction.Equals("up") || direction.Equals("down"))
                {
                    rot1 = Quaternion.AngleAxis(0, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(180, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot1) as GameObject;
                    rearBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.75f, 0), rot2) as GameObject;
                }

                straightBullet.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * shotSpeed;

                rearBullet.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                rearBullet.GetComponent<Rigidbody2D>().velocity = dir2.normalized * shotSpeed;
                break;

        }
           
    }

	//Esegue l'attacco meele
    public void Slash(string direction, Vector3 playerPos)
    {

        switch (direction)
        {
            case "up":
				//faccio partire l'animazione dell'attacco
                animator.Play("SlashUp"); 
				//calcolo la posizione da cui parte l'attacco
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + upDistance;
				//salvo un elenco dei collider trovati nell'area dell'attacco
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
            case "down":
                animator.Play("SlashDown");
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + downDistance;
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
            case "left":
                animator.Play("SlashLeft");
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + leftDistance;
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
            case "right":
                animator.Play("SlashRight");
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + rightDistance;
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
        }

		//se l'elenco dei collider trovati non è vuoto
        if (hitObjects != null)
        {
			//controllo ogni collider
            foreach (Collider2D coll in hitObjects)
            {
				//se trovo delle hitBox nemiche tra i collider
                if (coll.gameObject.tag == "Enemy" && coll.isTrigger)
                {
					//sparo un raycast verso il nemico per evitare di infliggere danno in caso ci siano ostacoli tra il player e il nemico
                    Vector3 dir = coll.gameObject.transform.position - (transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0));
                    RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0), dir);
					//se la via è libera infliggo danno al nemico
                    if (hit.collider.tag == "Enemy")
                    {
                        hitEnemy = true;
                        coll.gameObject.GetComponent<EnemyHealth>().TakeDamage(actualWeapon.damage);
                    }
                }

            }

            if (hitEnemy)
            {
                source.PlayOneShot(cut);
                hitEnemy = false;
            }
            else
            {
                source.PlayOneShot(swing);
            }
            hitObjects = null;
        }

    }

	//Genera quattro proiettili che vanno nelle quattro direzioni oblique
    public void SplitBullet(ItemStats weapon, Vector3 position)
    {
        Quaternion rot1 = Quaternion.AngleAxis(-45, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis(-135, Vector3.forward);
        Quaternion rot3 = Quaternion.AngleAxis(-225, Vector3.forward);
        Quaternion rot4 = Quaternion.AngleAxis(-315, Vector3.forward);
        Vector3 dir1 = rot1 * Vector3.up;
        Vector3 dir2 = rot2 * Vector3.up;
        Vector3 dir3 = rot3 * Vector3.up;
        Vector3 dir4 = rot4 * Vector3.up;
        float shotSpeed;

		//aggiorno la velocità dei proiettili se è attivo uno status speedUp o slowDown
        if (ph.faster)
            shotSpeed = weapon.shotSpeed + 3;
        else if (ph.slower)
            shotSpeed  = weapon.shotSpeed - 2;
        else
            shotSpeed = weapon.shotSpeed;

		//per creare i proiettili genero le statistiche dimezzate rispetto a quelle dell'arma equipaggiata dal player
        ItemStats newWeapon = new ItemStats(weapon.itemType, weapon.weaponType, ItemStats.FireType.single, weapon.bulletType, weapon.damage / 2, weapon.range / 2, weapon.fireRate, shotSpeed);

        GameObject bullet1 = Instantiate(bulletPrefab, position + new Vector3(0.3f, 0.3f, 0), rot1) as GameObject;
        bullet1.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet1.GetComponent<Rigidbody2D>().velocity = dir1.normalized * shotSpeed;
        bullet1.name = "b1";
        
        GameObject bullet2 = Instantiate(bulletPrefab, position + new Vector3(0.3f, -0.3f ,0), rot2) as GameObject;
        bullet2.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet2.GetComponent<Rigidbody2D>().velocity = dir2.normalized * shotSpeed;
        bullet2.name = "b2";

        GameObject bullet3 = Instantiate(bulletPrefab, position + new Vector3(-0.3f, -0.3f, 0), rot3) as GameObject;
        bullet3.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet3.GetComponent<Rigidbody2D>().velocity = dir3.normalized * shotSpeed;
        bullet3.name = "b3";

        GameObject bullet4 = Instantiate(bulletPrefab, position + new Vector3(-0.3f, 0.3f, 0), rot4) as GameObject;
        bullet4.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet4.GetComponent<Rigidbody2D>().velocity = dir4.normalized * shotSpeed;
        bullet4.name = "b4";

        actualRoom = GameManager.manager.ActualRoom;
        actualRoom.toSort.Add(bullet1);
        actualRoom.toSort.Add(bullet2);
        actualRoom.toSort.Add(bullet3);
        actualRoom.toSort.Add(bullet4);
    }

	//Disegna un cerchio nel raggio d'azione dell'attacco meele (viene visualizzato nell'editor)
    void OnDrawGizmos()
    {
        if (isAttacking)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(attackStart, radius);
        } 
    }

	//Utilizzata da Inventory per equipaggiare un'arma dell'inventario
    public void EquipWeapon(ItemStats weapon)
    {
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(weapon), actualWeapon);
        if (weapon.weaponType == ItemStats.WeaponType.meele)
        {
            source.PlayOneShot(unsheathe);
        }
        else
        {
            source.PlayOneShot(pickRanged);
        }

		attackTimer = attackDuration;
		shootTimer = actualWeapon.fireRate;
	}
}
