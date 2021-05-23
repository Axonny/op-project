public class CharacteristicPanelScripts
{
    public void IncreaseCharacteristic(int index)
    {
        Player.Instance.IncreaseCharacteristic(index);
    }

    public void DecreaseCharacteristic(int index)
    {
        Player.Instance.DecreaseCharacteristic(index);
    }

    public void SaveCharacteristics()
    {
        Player.Instance.SaveCharacteristics();
    }

    public void ResetCharacteristics()
    {
        Player.Instance.ResetCharacteristics();
    }

}