using Microsoft.AspNetCore.Mvc;



namespace MOM
{
    public class MeetingTypeController : Controller
    {

        public IActionResult GetAllMeetingType()
        {
           

            return View();
        }


        //add meeting type

        public IActionResult AddMeetingType()
        {
            //PR_MOM_MeetingType_Insert
            return View();
        }

        
        



        //delete meeting type
        public IActionResult DeleteMeetingType()
        {
            return View();

        }

        //edit meeting type
        public IActionResult EditMeetingType()
        {
            return View();
        }

        //select by id  meeting type
        public IActionResult GetMeetingTypeById()
        { 
             return View();
        }




       
    }

    
}
