//using TMPro;
//using UnityEngine;
//using Michsky;

//namespace Halabang.Blueberry.pp
//{
//    public class PhoneTelephoneController : PhoneAppController
//    {
//        private string phoneNumber = "";
//        [SerializeField]private TextMeshProUGUI phoneNumberDisplay;
//        [SerializeField] private TextMeshProUGUI callNumberDisplay;
//        // Start is called once before the first execution of Update after the MonoBehaviour is created
//        void Start()
//        {
//            PhoneManager.instance.phoneTelephoneManager.phoneTelephoneController = this;
//            phoneNumberDisplay.text = "";
//        }

//        // Update is called once per frame
//        void Update()
//        {

//        }
//        public void AppendNumber(string number)
//        {
//            phoneNumber += number;
//            phoneNumberDisplay.text = phoneNumber;
//            PhoneManager.instance.phoneTelephoneManager.currentCallContactNumber = phoneNumber;
//            callNumberDisplay.text = phoneNumber;
//            Debug.Log("Current Phone Number: " + phoneNumber);
//        }
//        public void ClearNumber()
//        {
//            phoneNumber = "";
//            phoneNumberDisplay.text = phoneNumber;
//            Debug.Log("Phone Number Cleared");
//        }
//    }
//}