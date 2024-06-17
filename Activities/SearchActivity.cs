using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace finalproject216303628
{
    [Activity(Label = "SearchActivity")]
    public class SearchActivity : AppCompatActivity
    {
        EditText etSearch;
        Button searchBtn;
        ListView searchLv;
        Spinner spAge, spMinAge, spMaxAge, spSubject, spGender, spFilters;

        SQLiteConnection dbCommand;
        ISharedPreferences sp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SearchBarLayout);

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            etSearch = FindViewById<EditText>(Resource.Id.etSearch);
            searchBtn = FindViewById<Button>(Resource.Id.searchBtn);
            searchLv = FindViewById<ListView>(Resource.Id.searchLv);
            spAge = FindViewById<Spinner>(Resource.Id.spAge);
            spMinAge = FindViewById<Spinner>(Resource.Id.spMinAge);
            spMaxAge = FindViewById<Spinner>(Resource.Id.spMaxAge);
            spSubject = FindViewById<Spinner>(Resource.Id.spSubject);
            spGender = FindViewById<Spinner>(Resource.Id.spGender);
            spFilters = FindViewById<Spinner>(Resource.Id.spFilters);

            string[] ageOptions = new string[2] { "Any Age", "Pick Age"}; 

            string[] agePickOptions = new string[84];
            for(int i = 16; i < 100; i++)
            {
                agePickOptions[i - 16] = i.ToString(); 
            }

            string[] filterOptions = Resources.GetStringArray(Resource.Array.searchOptions);

            string[] subjects = Resources.GetStringArray(Resource.Array.subjects);
            string[] subjectOptions = new string[subjects.Length + 1];
            subjectOptions[0] = "Any Subject";
            for(int i = 1; i < subjectOptions.Length; i++)
            {
                subjectOptions[i] = subjects[i - 1];
            }

            string[] genderOptions = new string[3] { "Any Gender", "Male", "Female" };

            ArrayAdapter agePickAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, agePickOptions);
            agePickAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);

            ArrayAdapter<string> ageAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, ageOptions);
            ageAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            ArrayAdapter<string> filterAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, filterOptions);
            filterAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            ArrayAdapter<string> genderAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, genderOptions);
            genderAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            ArrayAdapter<string> subjectAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, subjectOptions);
            subjectAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spAge.Adapter = ageAdapter;
            spMaxAge.Adapter = agePickAdapter;
            spMinAge.Adapter = agePickAdapter;
            spFilters.Adapter = filterAdapter;
            spGender.Adapter = genderAdapter;
            spSubject.Adapter = subjectAdapter;

            spAge.ItemSelected += SpAge_ItemSelected;
            searchBtn.Click += SearchBtn_Click;
        }

        private void SpAge_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (spAge.SelectedItem.ToString())
            {
                case "Any Age":
                    spMinAge.Visibility = ViewStates.Gone;
                    spMaxAge.Visibility = ViewStates.Gone;
                    break;
                case "Pick Age":
                    spMinAge.Visibility = ViewStates.Visible;
                    spMaxAge.Visibility = ViewStates.Visible;
                    break;
            }
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            try
            {

                if (etSearch.Text != "" && !Validation.IsOnlyLetters(etSearch.Text))
                    throw new Exception("Please Enter A Valid Input");

                string q = "";

                if(spSubject.SelectedItem.ToString() == "Any Subject")
                {
                    q += "SELECT * FROM TutorsTable";

                    switch (spFilters.SelectedItem.ToString())
                    {
                        case "Name":
                            q += $" WHERE FullName LIKE '%{etSearch.Text}%' COLLATE NOCASE";
                            break;
                        case "Email":
                            q += $" WHERE Email LIKE '%{etSearch.Text}%' COLLATE NOCASE";
                            break;
                    }

                    switch(spGender.SelectedItem.ToString())
                    {
                        case "Male":
                            q += $" AND Gender = '{spGender.SelectedItem.ToString()}'";
                            break;
                        case "Female":
                            q += $" AND Gender = '{spGender.SelectedItem.ToString()}'";
                            break;
                    }

                    if(spAge.SelectedItem.ToString() == "Pick Age")
                    {
                        q += $" AND BirthDate > '{new DateTime(DateTime.Now.Year - int.Parse(spMaxAge.SelectedItem.ToString()), DateTime.Now.Month, DateTime.Now.Day).Ticks}' AND BirthDate < '{new DateTime(DateTime.Now.Year - int.Parse(spMinAge.SelectedItem.ToString()), DateTime.Now.Month, DateTime.Now.Day).Ticks}'";
                    }

                    q += $" AND IsTeacher = 1 ORDER BY Rating DESC";
                }
                else
                {
                    q += $"SELECT TutorsTable.* FROM TutorsTable INNER JOIN Subjects ON TutorsTable.Email = Subjects.Email WHERE Subjects.Subject LIKE '{spSubject.SelectedItem.ToString()}'";

                    switch (spFilters.SelectedItem.ToString())
                    {
                        case "Name":
                            q += $" AND TutorsTable.FullName LIKE '%{etSearch.Text}%' COLLATE NOCASE";
                            break;
                        case "Email":
                            q += $" AND TutorsTable.Email LIKE '%{etSearch.Text}%' COLLATE NOCASE";
                            break;
                    }

                    switch (spGender.SelectedItem.ToString())
                    {
                        case "Male":
                            q += $" AND TutorsTable.Gender = '{spGender.SelectedItem.ToString()}'";
                            break;
                        case "Female":
                            q += $" AND TutorsTable.Gender = '{spGender.SelectedItem.ToString()}'";
                            break;
                    }

                    if (spAge.SelectedItem.ToString() == "Pick Age")
                    {
                        q += $" AND TutorsTable.BirthDate > '{new DateTime(DateTime.Now.Year - int.Parse(spMaxAge.SelectedItem.ToString()), DateTime.Now.Month, DateTime.Now.Day).Ticks}' AND TutorsTable.BirthDate < '{new DateTime(DateTime.Now.Year - int.Parse(spMinAge.SelectedItem.ToString()), DateTime.Now.Month, DateTime.Now.Day).Ticks}'";
                    }

                    q += $" AND TutorsTable.IsTeacher = 1 ORDER BY TutorsTable.Rating DESC";
                }

                List<Tutors> tutors = dbCommand.Query<Tutors>(q);

                TutorAdapter adapter = new TutorAdapter(this, tutors);

                searchLv.Adapter = adapter;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (sp.GetInt("IsTeacher", 0) == 0)
                MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
            else
                MenuInflater.Inflate(Resource.Menu.SearchMenu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MainPageMenuBtn:
                    StartActivity(new Intent(this, typeof(MainPageActivity)));
                    return true;
                case Resource.Id.UpdateWorkHoursMenuBtn:
                    StartActivity(new Intent(this, typeof(AddWorkHoursActivity)));
                    return true;
                case Resource.Id.ShowWorkHoursMenuBtn:
                    StartActivity(new Intent(this, typeof(ShowWorkHoursActivity)));
                    return true;
                case Resource.Id.UpdateInfoMenuBtn:
                    StartActivity(new Intent(this, typeof(UpdateUserInfo)));
                    return true;
                case Resource.Id.logoutMenuBtn:
                    bool keep = sp.GetBoolean("keep", false);
                    var editor = sp.Edit();
                    if (keep)
                    {
                        editor.PutString("name", "");
                        editor.PutString("gender", "");
                        editor.PutString("city", "");
                    }
                    else if (!keep)
                    {
                        editor.PutString("name", "");
                        editor.PutString("gender", "");
                        editor.PutString("city", "");
                        editor.PutString("email", "");
                        editor.PutString("pass", "");
                    }
                    editor.PutBoolean("loggedIn", false);
                    StartActivity(new Intent(this, typeof(LoginActivity)));
                    return true;
                case Resource.Id.ProfileMenuBtn:
                    Intent intent = new Intent(this, typeof(ProfileActivity));
                    intent.PutExtra("email", sp.GetString("email", ""));
                    StartActivity(intent);
                    return true;
                case Resource.Id.BecomeTeacherMenuBtn:
                    if (Helper.CalculateAge(Helper.DbCommand().Get<Tutors>(sp.GetString("email", "")).BirthDate) >= 16)
                        StartActivity(new Intent(this, typeof(AddWorkHoursActivity)));
                    else
                        Toast.MakeText(this, "Not Old Enough!", ToastLength.Short).Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}