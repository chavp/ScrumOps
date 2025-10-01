namespace ScrumOps.Domain.Metrics.ValueObjects;

/// <summary>
/// Types of metrics that can be tracked in the system.
/// </summary>
public enum MetricType
{
    // Team Performance Metrics
    TeamVelocity,
    TeamBurndown,
    TeamCapacityUtilization,
    TeamProductivity,
    
    // Sprint Metrics
    SprintBurndown,
    SprintCompletion,
    SprintScopeChange,
    SprintTaskCompletion,
    
    // Product Backlog Metrics
    BacklogRefinementRate,
    BacklogItemCycleTime,
    BacklogItemLeadTime,
    
    // Quality Metrics
    DefectRate,
    CodeCoverage,
    TechnicalDebt,
    
    // Process Metrics
    CeremoniesAttendance,
    PlanningAccuracy,
    EstimationAccuracy,
    
    // Individual Performance
    IndividualContribution,
    TaskCompletionRate,
    
    // Business Value
    ValueDelivered,
    CustomerSatisfaction,
    FeatureUsage
}

/// <summary>
/// Extensions for MetricType enum.
/// </summary>
public static class MetricTypeExtensions
{
    public static string GetDisplayName(this MetricType metricType) => metricType switch
    {
        MetricType.TeamVelocity => "Team Velocity",
        MetricType.TeamBurndown => "Team Burndown",
        MetricType.TeamCapacityUtilization => "Team Capacity Utilization",
        MetricType.TeamProductivity => "Team Productivity",
        MetricType.SprintBurndown => "Sprint Burndown",
        MetricType.SprintCompletion => "Sprint Completion",
        MetricType.SprintScopeChange => "Sprint Scope Change",
        MetricType.SprintTaskCompletion => "Sprint Task Completion",
        MetricType.BacklogRefinementRate => "Backlog Refinement Rate",
        MetricType.BacklogItemCycleTime => "Backlog Item Cycle Time",
        MetricType.BacklogItemLeadTime => "Backlog Item Lead Time",
        MetricType.DefectRate => "Defect Rate",
        MetricType.CodeCoverage => "Code Coverage",
        MetricType.TechnicalDebt => "Technical Debt",
        MetricType.CeremoniesAttendance => "Ceremonies Attendance",
        MetricType.PlanningAccuracy => "Planning Accuracy",
        MetricType.EstimationAccuracy => "Estimation Accuracy",
        MetricType.IndividualContribution => "Individual Contribution",
        MetricType.TaskCompletionRate => "Task Completion Rate",
        MetricType.ValueDelivered => "Value Delivered",
        MetricType.CustomerSatisfaction => "Customer Satisfaction",
        MetricType.FeatureUsage => "Feature Usage",
        _ => metricType.ToString()
    };

    public static string GetCategory(this MetricType metricType) => metricType switch
    {
        MetricType.TeamVelocity => "Team Performance",
        MetricType.TeamBurndown => "Team Performance",
        MetricType.TeamCapacityUtilization => "Team Performance",
        MetricType.TeamProductivity => "Team Performance",
        MetricType.SprintBurndown => "Sprint Metrics",
        MetricType.SprintCompletion => "Sprint Metrics",
        MetricType.SprintScopeChange => "Sprint Metrics",
        MetricType.SprintTaskCompletion => "Sprint Metrics",
        MetricType.BacklogRefinementRate => "Product Backlog",
        MetricType.BacklogItemCycleTime => "Product Backlog",
        MetricType.BacklogItemLeadTime => "Product Backlog",
        MetricType.DefectRate => "Quality",
        MetricType.CodeCoverage => "Quality",
        MetricType.TechnicalDebt => "Quality",
        MetricType.CeremoniesAttendance => "Process",
        MetricType.PlanningAccuracy => "Process",
        MetricType.EstimationAccuracy => "Process",
        MetricType.IndividualContribution => "Individual",
        MetricType.TaskCompletionRate => "Individual",
        MetricType.ValueDelivered => "Business Value",
        MetricType.CustomerSatisfaction => "Business Value",
        MetricType.FeatureUsage => "Business Value",
        _ => "Other"
    };

    public static string GetUnit(this MetricType metricType) => metricType switch
    {
        MetricType.TeamVelocity => "Story Points",
        MetricType.TeamBurndown => "Story Points",
        MetricType.TeamCapacityUtilization => "Percentage",
        MetricType.TeamProductivity => "Points/Hour",
        MetricType.SprintBurndown => "Story Points",
        MetricType.SprintCompletion => "Percentage",
        MetricType.SprintScopeChange => "Percentage",
        MetricType.SprintTaskCompletion => "Percentage",
        MetricType.BacklogRefinementRate => "Items/Sprint",
        MetricType.BacklogItemCycleTime => "Days",
        MetricType.BacklogItemLeadTime => "Days",
        MetricType.DefectRate => "Defects/Story Point",
        MetricType.CodeCoverage => "Percentage",
        MetricType.TechnicalDebt => "Hours",
        MetricType.CeremoniesAttendance => "Percentage",
        MetricType.PlanningAccuracy => "Percentage",
        MetricType.EstimationAccuracy => "Percentage",
        MetricType.IndividualContribution => "Story Points",
        MetricType.TaskCompletionRate => "Percentage",
        MetricType.ValueDelivered => "Business Value Points",
        MetricType.CustomerSatisfaction => "Score (1-10)",
        MetricType.FeatureUsage => "Usage Count",
        _ => "Value"
    };
}