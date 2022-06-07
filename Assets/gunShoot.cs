using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gunShoot : MonoBehaviour
{
    // gun stats
    public GameObject gun;
    public Transform attackPoint; // tip of gun
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    float reloadFrames, reloadProgress;

    // bullet stats
    public GameObject bullet;
    public float shootForce, upwardForce, bulletLife;

    // 'permissions'
    bool shooting, readyToShoot, reloading;

    // target aquisition
    public RaycastHit rayHit;

    // graphics
    public TextMeshProUGUI ammo;
    public Slider reloadSlider;

    private void Awake() {
        bulletsLeft = magSize;
        readyToShoot = true;
        reloadSlider.gameObject.SetActive(false);
    }

    private void Update() {
        if (allowButtonHold) {
            shooting = Input.GetKey(KeyCode.Mouse0);
        } else {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && !reloading) {
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0) {
            bulletsShot = 0;
            Shoot();
        }

        if (reloading) {
            reloadProgress++;
            reloadSlider.value = (reloadProgress / reloadFrames);
            ammo.SetText("Reloading!");
        }else if (ammo != null) {
            ammo.SetText(bulletsLeft + " / " + magSize);
        }
    }

    private void Shoot() {
        readyToShoot = false;

        // raycast target direction
        Ray ray = new Ray(attackPoint.transform.position, gun.transform.forward);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit)){
            targetPoint = hit.point;
        } else {
            targetPoint = ray.GetPoint(100); // literally just a point far from the player
        }

        // calc direction without spread
        Vector3 dirWithoutSpread = targetPoint - attackPoint.position;

        // calc spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // calc direction with spread
        Vector3 dirWithSpread = dirWithoutSpread + new Vector3(x, y, 0);

        // Instantiate and shoot bullet
        GameObject currBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currBullet.transform.forward = dirWithSpread.normalized;
        currBullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);
        currBullet.GetComponent<Rigidbody>().AddForce(gun.transform.up * upwardForce, ForceMode.Impulse);
        Destroy(currBullet, bulletLife);

        bulletsLeft--;
        bulletsShot++;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0) {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot() {
        readyToShoot = true;
    }

    private void Reload() {
        reloading = true;
        reloadFrames = 1.0f / Time.deltaTime * reloadTime;
        reloadProgress = 0;
        reloadSlider.value = 0;
        reloadSlider.gameObject.SetActive(true);
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished() {
        reloadSlider.gameObject.SetActive(false);
        bulletsLeft = magSize;
        reloading = false;
    }
}
