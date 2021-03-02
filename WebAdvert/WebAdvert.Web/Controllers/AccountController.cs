using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Account;

namespace WebAdvert.Web.Controllers 
{
  public class AccountController: Controller 
  {
    private readonly SignInManager<CognitoUser> _signInManager;
    private readonly CognitoUserManager<CognitoUser> _userManager;
    private readonly CognitoUserPool _pool;

    public AccountController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
    {
        _signInManager = signInManager;
        _userManager = userManager as CognitoUserManager<CognitoUser>;
        _pool = pool;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> SignUp() 
    {
      var model = new SignUpModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpModel model)
    {
      if (ModelState.IsValid) 
      {
        var user = _pool.GetUser(model.Email);

        if (user.Status != null)
          ModelState.AddModelError("UserExists", "User with this email already exists");
        
        user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

        var createdUser = await _userManager.CreateAsync(user, model.Password);

        if (createdUser.Succeeded)
        {
          return RedirectToAction("Confirm");
        }
      }

      return View(model);
    }

    public async Task<IActionResult> Confirm() 
    {
      var model = new ConfirmModel();
      return View(model); 
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(ConfirmModel model)
    {
      if (ModelState.IsValid) 
      {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if(user == null)
        {
          ModelState.AddModelError("Not Found", "A user with the given email address was not found");
          return View(model);
        }

        var result = await _userManager.ConfirmSignUpAsync(user, model.Code, true);

        if(result.Succeeded) 
        {
          return RedirectToAction("Index", "Home");
        }
        else 
        {
          foreach(var error in result.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }

          return View(model);
        }
      } 
      
      return View(model);
    }

    public async Task<IActionResult> Login()
    {
      var model = new LoginModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
      if (ModelState.IsValid)
      {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
          return RedirectToAction("Index", "Home");
        }
        else 
        {
          ModelState.AddModelError("LoginError", "Email and password do not match");
        }
      }

      return View("Login", model);
    }

    public async Task<IActionResult> ForgotPassword()
    {
      var model = new ForgotPasswordModel();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
          ModelState.AddModelError("Not Found", "A user with the given email address was not found");
          return View(model);
        }

        await user.ForgotPasswordAsync();
      }

      return View(model);
    }
  }
}