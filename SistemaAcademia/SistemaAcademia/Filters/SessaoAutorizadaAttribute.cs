using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaAcademia.Filters;

public class SessaoAutorizadaAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessao = context.HttpContext.Session;

        if (string.IsNullOrEmpty(sessao.GetString("TipoSessao")))
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}