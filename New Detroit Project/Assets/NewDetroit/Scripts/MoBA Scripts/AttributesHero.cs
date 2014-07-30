using UnityEngine;
using System.Collections;

public class AttributesHero : CLife
{

    // Variables
    protected int   maximunAdren = 100,
                    currentAdren,
                    maximunMana = 100,
                    currentMana,
                    deffenseMagic = 10,
                    deffensePhysic = 10,
                    level = 1,
                    attackPhysic = 100,
                    attackMagic = 100,
                    experience = 0,
                    speedWalk = 50,
                    speedRun = 50;
    protected float speedAttack = 1;

    // GUI variables
    private Rect rectanglePositiveLife,
                    rectangleNegativeLife,
                    rectanglePositiveAdren,
                    rectangleNegativeAdren,
                    rectanglePositiveMana,
                    rectangleNegativeMana,
                    rectangleLevel;
    public Texture2D    textureLifePositive, textureLifeNegative,
                        textureAdrenPositive, textureAdrenNegative,
                        textureManaPositive, textureManaNegative,
                        textureBackground;

    // Skills variables
    protected bool useSkill1, useSkill2, useSkill3;
    protected float currentCooldown1, currentCooldown2, currentCooldown3, maximunCooldown1, maximunCooldown2, maximunCooldown3;
    protected int manaSkill1, manaSkill2, manaSkill3, adrenSkill1, adrenSkill2, adrenSkill3; // mana and adrenalines for skills


    //-------------------------------------------------------


    public virtual void Start()
    {
        base.Start();

        useSkill1 = useSkill2 = useSkill3 = false;
    }

    public virtual void Update()
    {
        GUIRects();
    }

    public virtual void OnGUI()
    {
        GUI.DrawTexture(rectanglePositiveLife, textureLifePositive);
        GUI.DrawTexture(rectangleNegativeLife, textureLifeNegative);
        GUI.DrawTexture(rectanglePositiveAdren, textureAdrenPositive);
        GUI.DrawTexture(rectangleNegativeAdren, textureAdrenNegative);
        GUI.DrawTexture(rectanglePositiveMana, textureManaPositive);
        GUI.DrawTexture(rectangleNegativeMana, textureManaNegative);
        GUI.DrawTexture(rectangleLevel, textureBackground);

        FontStyle fs = GUI.skin.label.fontStyle;
        TextAnchor ta = GUI.skin.label.alignment;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(rectangleLevel, "" + level);
        GUI.skin.label.fontStyle = fs;
        GUI.skin.label.alignment = ta;
    }


    //-------------------------------------------------------


    private void GUIRects()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position), // real distance from camera
        //lengthLifeAdrenMana = this.GetComponent<ThirdPersonCamera>().distance / distance, // percentage of the distance
        lengthLifeAdrenMana = Camera.main.GetComponent<CameraMOBAController>().GetDistanceBack() / distance, // percentage of the distance
        widthAll = Screen.width / 10,
        widthHalf = widthAll / 2,
        positiveLife = (float)currentLife / maximunLife, // percentage of positive life
        positiveAdren = (float)currentAdren / maximunAdren, // percentage of positive adrenaline
        positiveMana = (float)currentMana / maximunMana; // percentage of positive mana
        // Life
        Vector3 posScene = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z),
        posSceneEnd = new Vector3(transform.position.x, transform.position.y + 1.8f, transform.position.z),
        pos = Camera.main.WorldToScreenPoint(posScene),
        posEnd = Camera.main.WorldToScreenPoint(posSceneEnd);

        float x = pos.x - widthHalf * lengthLifeAdrenMana,
        y = Screen.height - pos.y,
        width = widthAll * positiveLife * lengthLifeAdrenMana,
        height = (pos.y - posEnd.y) * lengthLifeAdrenMana;
        rectanglePositiveLife = new Rect(x, y, width, height);

        x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveLife * lengthLifeAdrenMana;
        width = widthAll * (1 - positiveLife) * lengthLifeAdrenMana;
        rectangleNegativeLife = new Rect(x, y, width, height);
        // Adrenaline
        posScene = new Vector3(transform.position.x, transform.position.y + 1.78f, transform.position.z);
        posSceneEnd = new Vector3(transform.position.x, transform.position.y + 1.68f, transform.position.z);
        pos = Camera.main.WorldToScreenPoint(posScene);
        posEnd = Camera.main.WorldToScreenPoint(posSceneEnd);

        x = pos.x - widthHalf * lengthLifeAdrenMana;
        y = Screen.height - pos.y;
        width = widthAll * positiveAdren * lengthLifeAdrenMana;
        height = (pos.y - posEnd.y) * lengthLifeAdrenMana;
        rectanglePositiveAdren = new Rect(x, y, width, height);

        x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveAdren * lengthLifeAdrenMana;
        width = widthAll * (1 - positiveAdren) * lengthLifeAdrenMana;
        rectangleNegativeAdren = new Rect(x, y, width, height);
        // Mana
        posScene = new Vector3(transform.position.x, transform.position.y + 1.66f, transform.position.z);
        posSceneEnd = new Vector3(transform.position.x, transform.position.y + 1.56f, transform.position.z);
        pos = Camera.main.WorldToScreenPoint(posScene);
        posEnd = Camera.main.WorldToScreenPoint(posSceneEnd);

        x = pos.x - widthHalf * lengthLifeAdrenMana;
        y = Screen.height - pos.y;
        width = widthAll * positiveMana * lengthLifeAdrenMana;
        height = (pos.y - posEnd.y) * lengthLifeAdrenMana;
        rectanglePositiveMana = new Rect(x, y, width, height);

        x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveMana * lengthLifeAdrenMana;
        width = widthAll * (1 - positiveMana) * lengthLifeAdrenMana;
        rectangleNegativeMana = new Rect(x, y, width, height);
        // Level
        posScene = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        posSceneEnd = new Vector3(transform.position.x, transform.position.y + 1.56f, transform.position.z);
        pos = Camera.main.WorldToScreenPoint(posScene);
        posEnd = Camera.main.WorldToScreenPoint(posSceneEnd);

        x = pos.x - (widthHalf + 22) * lengthLifeAdrenMana;
        y = rectanglePositiveLife.y;
        width = 20 * lengthLifeAdrenMana;
        height = (rectanglePositiveMana.y - rectanglePositiveLife.y) + rectanglePositiveMana.height;
        rectangleLevel = new Rect(x, y, width, height);

    }


    //-------------------------------------------------------


    [RPC]
    public void UpdateAdren(int amount)
    {
        currentAdren += amount;
        if (currentAdren < 0) currentAdren = 0;
        if (currentAdren > maximunAdren) currentAdren = maximunAdren;
    }

    [RPC]
    public void UpdateMana(int amount)
    {
        currentMana += amount;
        if (currentMana < 0) currentMana = 0;
        if (currentMana > maximunMana) currentMana = maximunMana;
    }

    [RPC]
    public void UpdateCooldown1(float amount)
    {
        currentCooldown1 += amount;
        if (currentCooldown1 < 0) currentCooldown1 = 0;
        if (currentCooldown1 > maximunCooldown1) currentCooldown1 = maximunCooldown1;
    }

    [RPC]
    public void UpdateCooldown2(float amount)
    {
        currentCooldown2 += amount;
        if (currentCooldown2 < 0) currentCooldown2 = 0;
        if (currentCooldown2 > maximunCooldown2) currentCooldown2 = maximunCooldown2;
    }

    [RPC]
    public void UpdateCooldown3(float amount)
    {
        currentCooldown3 += amount;
        if (currentCooldown3 < 0) currentCooldown3 = 0;
        if (currentCooldown3 > maximunCooldown3) currentCooldown3 = maximunCooldown3;
    }


    //-------------------------------------------------------


    public bool consumeAdren(int adren, int skill)
    {
        if (skill == 1 && adrenSkill1 == -1 ||
            skill == 2 && adrenSkill2 == -1 ||
            skill == 3 && adrenSkill3 == -1)
            return true;
        if (adren > currentAdren) return false;

        if (PhotonNetwork.offlineMode) UpdateAdren(-adren);
        else photonView.RPC("UpdateAdren", PhotonTargets.All, -adren);

        return true;
    }

    private bool CanConsumeAdren(int value, int skill)
    {
        if (skill == 1 && adrenSkill1 == -1 ||
            skill == 2 && adrenSkill2 == -1 ||
            skill == 3 && adrenSkill3 == -1)
            return true;

        return value <= currentAdren;
    }

    public bool consumeMana(int mana, int skill)
    {
        if (skill == 1 && manaSkill1 == -1 ||
            skill == 2 && manaSkill2 == -1 ||
            skill == 3 && manaSkill3 == -1)
            return true;
        if (mana > currentMana) return false;

        if (PhotonNetwork.offlineMode) UpdateMana(-mana);
        else photonView.RPC("UpdateMana", PhotonTargets.All, -mana);

        return true;
    }

    private bool CanConsumeMana(int value, int skill)
    {
        if (skill == 1 && manaSkill1 == -1 ||
            skill == 2 && manaSkill2 == -1 ||
            skill == 3 && manaSkill3 == -1)
            return true;

        return value <= currentMana;
    }

    public void recoverAdren(int adren)
    {
        if (currentAdren < maximunAdren)
        {
            if (PhotonNetwork.connected) photonView.RPC("UpdateAdren", PhotonTargets.All, adren);
            else  UpdateAdren(adren);
        }
    }

    public void recoverMana(int mana)
    {
        if (currentMana < maximunMana)
        {
            if (PhotonNetwork.connected) photonView.RPC("UpdateMana", PhotonTargets.All, mana);
            else UpdateMana(mana);
        }
    }

    public bool consumeCooldown(float value, int skill)
    {
        if (skill == 1 && currentCooldown1 > 0 ||
            skill == 2 && currentCooldown2 > 0 ||
            skill == 3 && currentCooldown3 > 0)
            return false;

        if (PhotonNetwork.offlineMode)
        {
            if (skill == 1) UpdateCooldown1(value);
            else if (skill == 2) UpdateCooldown2(value);
            else if (skill == 3) UpdateCooldown3(value);
        }
        else
        {
            if (skill == 1) photonView.RPC("UpdateCooldown1", PhotonTargets.All, value);
            else if (skill == 2) photonView.RPC("UpdateCooldown2", PhotonTargets.All, value);
            else if (skill == 3) photonView.RPC("UpdateCooldown3", PhotonTargets.All, value);
        }

        return true;
    }

    private bool CanConsumeCooldown(float value, int skill)
    {
        return  skill == 1 && currentCooldown1 <= 0 ||
                skill == 2 && currentCooldown2 <= 0 ||
                skill == 3 && currentCooldown3 <= 0;
    }

    public void recoverCooldown(float value)
    {
        if (currentCooldown1 > 0)
        {
            if (PhotonNetwork.offlineMode) UpdateCooldown1(-value);
            else photonView.RPC("UpdateCooldown1", PhotonTargets.All, -value);
        }

        if (currentCooldown2 > 0)
        {
            if (PhotonNetwork.offlineMode) UpdateCooldown2(-value);
            else photonView.RPC("UpdateCooldown2", PhotonTargets.All, -value);
        }

        if (currentCooldown3 > 0)
        {
            if (PhotonNetwork.offlineMode) UpdateCooldown3(-value);
            else photonView.RPC("UpdateCooldown3", PhotonTargets.All, -value);
        }
    }

    public bool UseSkill1()
    {
        if (!CanConsumeCooldown(maximunCooldown1, 1) || !CanConsumeMana(manaSkill1, 1) || !CanConsumeAdren(adrenSkill1, 1)) return false;

        return useSkill1 && consumeCooldown(maximunCooldown1, 1) && consumeMana(manaSkill1, 1) && consumeAdren(adrenSkill1, 1);
    }

    public bool UseSkill2()
    {
        if (!CanConsumeCooldown(maximunCooldown2, 2) || !CanConsumeMana(manaSkill2, 2) || !CanConsumeAdren(adrenSkill2, 2)) return false;

        return useSkill2 && consumeCooldown(maximunCooldown2, 2) && consumeMana(manaSkill2, 2) && consumeAdren(adrenSkill2, 2);
    }

    public bool UseSkill3()
    {
        if (!CanConsumeCooldown(maximunCooldown3, 3) || !CanConsumeMana(manaSkill3, 3) || !CanConsumeAdren(adrenSkill3, 3)) return false;

        return useSkill3 && consumeCooldown(maximunCooldown3, 3) && consumeMana(manaSkill3, 3) && consumeAdren(adrenSkill3, 3);
    }

    //-------------------------------------------------------


    public void setMaximunAdren(int adren) { maximunAdren = adren; }
    public int getMaximunAdren() { return maximunAdren; }
    public void setMaximunMana(int mana) { maximunMana = mana; }
    public int getMaximunMana() { return maximunMana; }
    public void setCurrentAdren(int adren) { currentAdren = adren; }
    public int getCurrentAdren() { return currentAdren; }
    public void setCurrentMana(int mana) { currentMana = mana; }
    public int getCurrentMana() { return currentMana; }
    public void setLevel(int level) { this.level = level; }
    public int getLevel() { return level; }
    public void setDeffenseMagic(int def) { deffenseMagic = def; }
    public int getDeffenseMagic() { return deffenseMagic; }
    public void setDeffensePhysic(int def) { deffensePhysic = def; }
    public int getDeffensePhysic() { return deffensePhysic; }
    public void setAttackPhysic(int value) { attackPhysic = value; }
    public int getAttackPhysic() { return attackPhysic; }
    public void setAttackMagic(int value) { attackMagic = value; }
    public int getAttackMagic() { return attackMagic; }
    public void setSpeedWalk(int value) { speedWalk = value; }
    public int getSpeedWalk() { return speedWalk; }
    public void setSpeedRun(int value) { speedRun = value; }
    public int getSpeedRun() { return speedRun; }
    public void setSpeedAttack(float value) { speedAttack = value; }
    public float getSpeedAttack() { return speedAttack; }
    public void setExperience(int value) { experience = value; }
    public int getExperience() { return experience; }
    public void setUseSkill1(bool value) { useSkill1 = value; }
    public bool getUseSkill1() { return useSkill1; }
    public void setUseSkill2(bool value) { useSkill2 = value; }
    public bool getUseSkill2() { return useSkill2; }
    public void setUseSkill3(bool value) { useSkill3 = value; }
    public bool getUseSkill3() { return useSkill3; }
    public void setCurrentCooldown1(float value) { currentCooldown1 = value; }
    public float getCurrentCooldown1() { return currentCooldown1; }
    public void setMaximunCooldown1(float value) { maximunCooldown1 = value; }
    public float getMaximunCooldown1() { return maximunCooldown1; }
    public void setCurrentCooldown2(float value) { currentCooldown2 = value; }
    public float getCurrentCooldown2() { return currentCooldown2; }
    public void setMaximunCooldown2(float value) { maximunCooldown2 = value; }
    public float getMaximunCooldown2() { return maximunCooldown2; }
    public void setCurrentCooldown3(float value) { currentCooldown3 = value; }
    public float getCurrentCooldown3() { return currentCooldown3; }
    public void setMaximunCooldown3(float value) { maximunCooldown3 = value; }
    public float getMaximunCooldown3() { return maximunCooldown3; }
    public void setManaSkill1(int value) { manaSkill1 = value; }
    public int getManaSkill1() { return manaSkill1; }
    public void setManaSkill2(int value) { manaSkill2 = value; }
    public int getManaSkill2() { return manaSkill2; }
    public void setManaSkill3(int value) { manaSkill3 = value; }
    public int getManaSkill3() { return manaSkill3; }
    public void setAdrenSkill1(int value) { adrenSkill1 = value; }
    public int getAdrenSkill1() { return adrenSkill1; }
    public void setAdrenSkill2(int value) { adrenSkill2 = value; }
    public int getAdrenSkill2() { return adrenSkill2; }
    public void setAdrenSkill3(int value) { adrenSkill3 = value; }
    public int getAdrenSkill3() { return adrenSkill3; }
}
