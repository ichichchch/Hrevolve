import { request } from '../request';
import type { 
  ShiftTemplate, Schedule, ScheduleProject,
  PageRequest, PageResponse 
} from '@/types';

/** 排班管理 API */
export const scheduleApi = {
  // ==================== 班次模板 ====================
  
  /** 获取班次模板列表 */
  getShiftTemplates(params?: { isActive?: boolean }) {
    return request.get<ShiftTemplate[]>('/schedules/shift-templates', { params });
  },
  
  /** 获取班次模板详情 */
  getShiftTemplate(id: string) {
    return request.get<ShiftTemplate>(`/schedules/shift-templates/${id}`);
  },
  
  /** 创建班次模板 */
  createShiftTemplate(data: Partial<ShiftTemplate>) {
    return request.post<ShiftTemplate>('/schedules/shift-templates', data);
  },
  
  /** 更新班次模板 */
  updateShiftTemplate(id: string, data: Partial<ShiftTemplate>) {
    return request.put<ShiftTemplate>(`/schedules/shift-templates/${id}`, data);
  },
  
  /** 删除班次模板 */
  deleteShiftTemplate(id: string) {
    return request.delete(`/schedules/shift-templates/${id}`);
  },
  
  // ==================== 排班记录 ====================
  
  /** 获取排班列表 */
  getSchedules(params: PageRequest & { 
    departmentId?: string; 
    employeeId?: string;
    startDate: string;
    endDate: string;
  }) {
    return request.get<PageResponse<Schedule>>('/schedules', { params });
  },
  
  /** 获取员工排班日历 */
  getEmployeeScheduleCalendar(employeeId: string, year: number, month: number) {
    return request.get<Schedule[]>(`/schedules/employee/${employeeId}/calendar`, {
      params: { year, month }
    });
  },
  
  /** 获取部门排班日历 */
  getDepartmentScheduleCalendar(departmentId: string, year: number, month: number) {
    return request.get<Schedule[]>(`/schedules/department/${departmentId}/calendar`, {
      params: { year, month }
    });
  },
  
  /** 创建排班 */
  createSchedule(data: Partial<Schedule>) {
    return request.post<Schedule>('/schedules', data);
  },
  
  /** 批量创建排班 */
  batchCreateSchedules(data: {
    employeeIds: string[];
    shiftTemplateId: string;
    dates: string[];
  }) {
    return request.post<Schedule[]>('/schedules/batch', data);
  },
  
  /** 更新排班 */
  updateSchedule(id: string, data: Partial<Schedule>) {
    return request.put<Schedule>(`/schedules/${id}`, data);
  },
  
  /** 删除排班 */
  deleteSchedule(id: string) {
    return request.delete(`/schedules/${id}`);
  },
  
  /** 复制排班 */
  copySchedules(data: {
    sourceStartDate: string;
    sourceEndDate: string;
    targetStartDate: string;
    departmentId?: string;
    employeeIds?: string[];
  }) {
    return request.post('/schedules/copy', data);
  },
  
  // ==================== 排班项目 ====================
  
  /** 获取排班项目列表 */
  getScheduleProjects(params?: PageRequest & { status?: string }) {
    return request.get<PageResponse<ScheduleProject>>('/schedules/projects', { params });
  },
  
  /** 获取排班项目详情 */
  getScheduleProject(id: string) {
    return request.get<ScheduleProject>(`/schedules/projects/${id}`);
  },
  
  /** 创建排班项目 */
  createScheduleProject(data: Partial<ScheduleProject>) {
    return request.post<ScheduleProject>('/schedules/projects', data);
  },
  
  /** 更新排班项目 */
  updateScheduleProject(id: string, data: Partial<ScheduleProject>) {
    return request.put<ScheduleProject>(`/schedules/projects/${id}`, data);
  },
  
  /** 发布排班项目 */
  publishScheduleProject(id: string) {
    return request.post(`/schedules/projects/${id}/publish`);
  },
  
  /** 删除排班项目 */
  deleteScheduleProject(id: string) {
    return request.delete(`/schedules/projects/${id}`);
  },
  
  // ==================== 排班统计 ====================
  
  /** 获取排班概览统计 */
  getScheduleOverview(params: { startDate: string; endDate: string; departmentId?: string }) {
    return request.get<{
      totalEmployees: number;
      scheduledEmployees: number;
      byShift: { shiftName: string; count: number; color: string }[];
      byDepartment: { departmentName: string; count: number }[];
      byLocation: { location: string; count: number }[];
    }>('/schedules/overview', { params });
  },
  
  /** 获取排班统计 */
  getScheduleStats() {
    return request.get<{
      totalEmployees: number;
      scheduledToday: number;
      onDuty: number;
      pendingApprovals: number;
    }>('/schedules/stats');
  },
  
  /** 获取可排班员工 */
  getSchedulableEmployees(params?: { departmentId?: string }) {
    return request.get<{ id: string; name: string; departmentId: string; departmentName: string }[]>(
      '/schedules/schedulable-employees', { params }
    );
  },
  
  /** 分配排班 */
  assignSchedule(data: { employeeId: string; date: string; shiftTemplateId: string | null }) {
    return request.post('/schedules/assign', data);
  },
};
