using UnityEngine;
using UnityEngine.UI;
using TimeOfDayURP;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    public TimeManager timeManager;
    public Text legacyTimeText;
    public TextMeshProUGUI tmpTimeText;
    public Text legacyDayText;
    public TextMeshProUGUI tmpDayText;
    public Text legacySeasonText;
    public TextMeshProUGUI tmpSeasonText;
    public bool use24HourFormat = false;

    void Update()
    {
        float timeOfDay = timeManager.timeOfDay;
        int hours = Mathf.FloorToInt(timeOfDay * 24);
        int minutes = Mathf.FloorToInt((timeOfDay * 24 - hours) * 60);

        // Get current day of the year and season from TimeManager
        int dayOfYear = timeManager.GetCurrentDayOfYear();
        string season = timeManager.GetCurrentSeason();

        string timeString = "";

        if (use24HourFormat)
        {
            timeString = string.Format("{0:D2}:{1:D2}", hours, minutes);
        }
        else
        {
            string amPm = hours >= 12 ? "PM" : "AM";
            hours = hours % 12;
            if (hours == 0) // 0 hours is 12 AM or PM
            {
                hours = 12;
            }
            timeString = string.Format("{0:D2}:{1:D2} {2}", hours, minutes, amPm);
        }

        // If both are set, TMP text will be preferred
        if (tmpTimeText != null)
        {
            tmpTimeText.text = timeString;
        }
        else if (legacyTimeText != null)
        {
            legacyTimeText.text = timeString;
        }

        // Display day of the year
        if (tmpDayText != null)
        {
            tmpDayText.text = dayOfYear.ToString();
        }
        else if (legacyDayText != null)
        {
            legacyDayText.text = dayOfYear.ToString();
        }

        // Display season
        if (tmpSeasonText != null)
        {
            tmpSeasonText.text = season;
        }
        else if (legacySeasonText != null)
        {
            legacySeasonText.text = season;
            legacySeasonText.text = season;
        }
    }
}
