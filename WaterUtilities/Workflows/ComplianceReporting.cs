using System.Dynamic;
using System.Net;
using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Models;

namespace WaterUtilities.Workflows;

public class ComplianceReporting : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var routeDataVariable = builder.WithVariable<IDictionary<string, object>>();
        var userIdVariable = builder.WithVariable<string>();
        var userVariable = builder.WithVariable<ExpandoObject>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new Input<string>("users/{userid}"),
                    SupportedMethods = new Input<ICollection<string>>([HttpMethods.Get]),
                    CanStartWorkflow = true,
                    RouteData = new Output<IDictionary<string, object>>(routeDataVariable)
                },
                new SetVariable
                {
                    Variable = userIdVariable,
                    Value = new Input<object?>(context =>
                    {
                        var routeData = routeDataVariable.Get(context)!;
                        var userId = routeData["userid"].ToString();
                        return userId;
                    })
                },
                new SendHttpRequest
                {
                    Url = new Input<Uri?>(context =>
                    {
                        var userId = userIdVariable.Get(context);
                        return new Uri($"https://reqres.in/api/users/{userId}");
                    }),
                    Method = new Input<string>(HttpMethods.Get),
                    ParsedContent = new Output<object?>(userVariable),
                    ExpectedStatusCodes =
                    {
                        new HttpStatusCodeCase
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Activity = new WriteHttpResponse
                            {
                                Content = new Input<object?>(context =>
                                {
                                    var user = (dynamic)userVariable.Get(context)!;
                                    return user.data;
                                }),
                                StatusCode = new Input<HttpStatusCode>(HttpStatusCode.OK)
                            }
                        },
                        new HttpStatusCodeCase
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            Activity = new WriteHttpResponse
                            {
                                Content = new Input<object?>("User not found"),
                                StatusCode = new Input<HttpStatusCode>(HttpStatusCode.NotFound)
                            }
                        }
                    }
                }
            }
        };
    }
}