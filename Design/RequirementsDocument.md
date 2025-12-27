# 企业级SaaS人力资源管理系统 - 需求文档

---

## 1. 引言 (Introduction)

### 1.1. 文档目的 (Purpose)

本文档旨在详细描述企业级SaaS人力资源管理（HRM）系统的功能性与非功能性需求，作为后续系统架构设计、开发、测试及项目验收的核心依据。

### 1.2. 项目范围 (Scope)

本项目旨在实现一套集成的、现代化的SaaS人力资源管理平台。核心功能模块包括：

- **基础模块**：灵活的组织架构管理、支持“生效日期”的员工全生命周期管理。
- **核心应用模块**：用户认证与安全（含单点登录与MFA）、考勤与排班管理、假期管理、薪酬核算与报销管理、通知系统。
- **数据智能模块**：人力资本分析仪表盘、基于RAG的AI助手、员工高离职风险预测模型。
- **集成能力**：提供标准化的API、Webhook及SCIM支持，以实现与企业现有ERP、OA等系统的无缝对接。

**本次范围不包括**：完整的招聘管理（ATS）、绩效考核、培训与发展模块，但设计上应为未来集成预留接口。

### 1.3. 定义与缩写 (Definitions and Acronyms)

| 缩写 | 全称 | 说明 |
|------|------|------|
| SaaS | Software as a Service | 软件即服务 |
| SSO | Single Sign-On | 单点登录 |
| MFA | Multi-Factor Authentication | 多因素认证 |
| RAG | Retrieval-Augmented Generation | 检索增强生成 |
| LLM | Large Language Model | 大型语言模型 |
| RBAC | Role-Based Access Control | 基于角色的访问控制 |
| RLS | Row-Level Security | 行级安全策略 |
| SCD Type 2 | Slowly Changing Dimension Type 2 | 第二类慢速变化维度 |
| SCIM | System for Cross-domain Identity Management | 跨域身份管理系统 |
| SMOTE | Synthetic Minority Over-sampling Technique | 处理数据不平衡的过采样技术 |
| SHAP | SHapley Additive exPlanations | 解释机器学习模型预测的工具 |

---

## 2. 整体描述 (Overall Description)

### 2.1. 产品愿景 (Product Perspective)

本产品是一个全新的、面向现代化企业的SaaS HR管理平台。它旨在通过数字化和自动化，整合企业核心人事流程，提升管理效率。更重要的是，系统将深度融合AI、数据分析与卓越的员工体验（EX），从传统的“管理工具”转变为“员工服务平台”和“企业战略决策引擎”。

### 2.2. 产品功能摘要 (Summary of Functions)

系统将提供：

- 灵活的组织架构管理
- 基于“生效日期”的员工全生命周期管理
- 多渠道安全登录（SSO、MFA、微信扫码）
- 智能排班与自动化考勤
- 可配置的假期策略
- 基于规则引擎的薪酬核算
- 全流程线上化费用报销
- 基于RAG的AI问答与业务办理助手
- 员工离职风险预测等高级智能服务

### 2.3. 用户特征 (User Characteristics)

系统主要用户包括：

- HR管理员
- HRBP（人力资源业务伙伴）
- 薪酬专员
- 财务人员
- 部门经理
- 普通员工

所有用户均具备基本计算机操作能力。

### 2.4. 约束与假设 (Constraints and Assumptions)

#### 2.4.1. 多租户架构与透明数据隔离策略

作为SaaS服务的基石，采用**混合多租户架构**，根据客户规模与安全要求动态选择隔离策略。

##### 为中小型客户（共享数据库、共享Schema）：

- **租户上下文管理**：
  - 使用 `ThreadLocal` 存储当前请求的 `tenant_id`。
  - 请求入口通过过滤器解析并验证 `tenant_id`，存入 `ThreadLocal`。
  - 请求结束时在 `finally` 块中执行 `remove()`，防止线程复用导致数据污染。

- **框架层自动化过滤（Query Rewriting）**：
  - **MyBatis/MyBatis-Plus**：实现 `TenantLineHandler` 接口，配置 `MybatisPlusInterceptor`，自动为SQL追加 `tenant_id` 条件。
  - **JPA/Hibernate**：使用 `@FilterDef` + `@Filter` 定义租户过滤器，通过AOP在Repository调用前启用并注入 `tenantId`。

- **查询重写需处理的边缘情况**：
  - 复杂 `JOIN`：确保所有关联表均附加 `tenant_id`。
  - 子查询与 `UNION`：递归解析并注入过滤条件。
  - 表别名：重写机制需正确解析并保留别名语义。

- **数据库层增强（RLS）**：
  - 启用数据库原生行级安全（如 PostgreSQL RLS、SQL Server Row-Level Security）作为第二道防线。

- **租户级加密**：
  - 对身份证、薪资等极度敏感字段，采用应用层加密（如 AES-GCM），并为每个租户使用独立加密密钥（Per-Tenant Key）。

##### 为大型或高安全要求客户（独立数据库）：

- 预留独立数据库部署能力，实现物理隔离，满足金融、政府等高合规场景需求。

---

## 3. 功能性需求 (Functional Requirements)

### 3.1. 组织与员工管理（核心基础）

#### 3.1.1. 灵活的树状组织架构

- **技术实现**：
  - 单表 `OrganizationUnits`。
  - 结合 **邻接表模型**（`parent_id`）与 **路径枚举模型**（`path`，如 `/1/5/23/`）。
  - 支持快速构建树、查询子树、判断上下级关系。

#### 3.1.2. 员工全生命周期管理与历史追溯（Effective Dating）

- **需求**：支持员工从入职到离职的全流程管理，可精确查询任意历史时间点的状态。

- **技术实现（SCD Type 2）**：
  - 主表：`JobHistory`
    - 字段：`employee_id`, `effective_start_date`, `effective_end_date`, `position_id`, `dept_id`, `salary`, `correction_status`
    - `effective_end_date` 默认为 `9999-12-31` 表示当前记录。

- **数据维护规则**：
  - 每次变更（调岗、调薪等）需在**同一事务**中：
    1. 更新旧记录的 `effective_end_date = 当前日期 - 1天`
    2. 插入新记录，`effective_start_date = 当前日期`

- **时点查询**：
  ```sql
  SELECT * FROM JobHistory 
  WHERE employee_id = ? 
    AND '2023-05-15' BETWEEN effective_start_date AND effective_end_date
    AND (correction_status IS NULL OR correction_status != 'Voided');
  ```

- **历史数据修正（冲正与补偿）**：
  - 错误记录标记为 `correction_status = 'Voided'`（保留审计痕迹）。
  - 插入新记录，使用**完全相同的** `effective_start_date` 和 `effective_end_date`。
  - 所有报表查询需过滤 `WHERE correction_status != 'Voided'`。

---

### 3.2. 用户认证与安全

- **3.2.1. 单点登录 (SSO)**：
  - 支持 OAuth 2.0 Authorization Code Flow + PKCE。
  - 兼容 Google、Microsoft Entra ID（Azure AD）、钉钉、飞书等主流 IdP。

- **3.2.2. 微信扫码登录**：
  - 后端生成带唯一 `scene_id` 的二维码。
  - 前端通过 WebSocket 或长轮询监听登录状态变更。

- **3.2.3. 多因素认证 (MFA)**：
  - 支持 TOTP（Google Authenticator）、短信、邮件验证码。
  - 提供设备信任、账户恢复流程（如备用代码）。

---

### 3.3. 考勤与排班管理

- 支持自定义班次（早班、晚班、弹性工时等）。
- 可视化拖拽排班界面。
- 支持多源打卡（App GPS、WiFi、考勤机、Web 手动补卡）。
- 自动核算工时、加班、缺勤。

---

### 3.4. 假期管理策略引擎

- 支持多种假期类型（年假、病假、产假、调休假等）。
- 可配置规则：
  - 入职后生效时间
  - 年度额度分配逻辑（按职级、司龄）
  - 结转规则（最多结转X天）
  - 过期清零策略
- 自动计算假期余额，支持审批流。

---

### 3.5. 薪酬与报销管理

#### 薪酬计算引擎

- **核心架构**：计算模型与执行分离。
- **规则定义中心**：可视化配置薪资项（基本工资、绩效、津贴、扣款等）及计算公式。
- **规则引擎**：集成 **Drools**，以 `.drl` 文件或决策表管理复杂逻辑（如个税、社保、奖金）。
- **执行机制**：
  - 通过 XXL-Job 触发批量任务。
  - 使用 Spring Batch 分片并行处理。
- **性能与准确性保障**：
  - 单员工计算：预加载所需数据至内存。
  - 缓存租户级规则。
  - 事务性 + 幂等性设计。
  - 详细计算日志（含数据快照）。
  - 支持“试算”与“正式计算锁定”。

#### 报销管理
- 全流程线上化：申请 → 附件上传 → 审批 → 财务打款。
- 与薪酬系统联动（如报销款计入当月工资）。

---

### 3.6. 数据分析与智能洞察

#### 3.6.1. 员工高离职风险预测

- **功能**：每月运行模型，在管理者仪表盘高亮高风险员工。
- **特征数据**：
  - 岗位、职级、司龄
  - 薪酬分位、调薪频率
  - 考勤异常、加班强度
  - 假期使用率、系统活跃度
  - （可选）匿名调研情绪得分

- **技术实现**：
  - **数据准备**：SMOTE 处理正样本稀疏问题。
  - **模型选型**：XGBoost / LightGBM（主），逻辑回归（基线）。
  - **评估指标**：Precision、Recall、F1、AUC-ROC。
  - **可解释性**：SHAP 值解释关键驱动因素。
  - **部署**：封装为 REST API，支持定期再训练与A/B测试。

#### 3.6.2. AI 助手（RAG-based HR Bot）

- **功能**：7×24 小时回答员工问题、办理自助服务（如请假、查余额）。
- **技术架构**：
  - **知识库**：
    - 非结构化：政策文档 → ETL → Chunking → Embedding → 向量库（Pinecone/ChromaDB）
    - 结构化：个人数据（假期、薪酬）→ 封装为安全API
  - **意图识别与工具调用**：
    - LLM 识别“我要请假” → 调用 `POST /api/leave-requests`
    - 支持多轮对话补全（如“请几天？”、“原因？”）
  - **安全**：
    - 所有API调用均在后端执行RBAC+RLS校验。
    - LLM **不持有**用户数据，仅传递意图。

---

## 4. 非功能性需求 (Non-Functional Requirements)

### 4.1. 性能
- 核心页面加载时间 < 2 秒（P95）
- 薪酬批量计算：10万员工 < 30 分钟
- 支持 10,000+ 并发用户

### 4.2. 安全性
- 传输层：强制 HTTPS（TLS 1.3）
- 存储层：敏感字段加密（AES-256），支持 Per-Tenant Key
- 访问控制：RBAC + 组织汇报线 + 多租户隔离（Query Rewriting + RLS）
- 审计日志：记录操作人、租户ID、时间、IP、操作详情，保留 ≥ 180 天

### 4.3. 可用性与员工体验
- 界面设计：消费级体验，响应式布局，移动端优先
- 一体化工作流：入职 → 开通账号 → 首次登录引导 → AI助手欢迎
- 支持多语言（中/英）

### 4.4. 可靠性与集成性

#### 4.4.1. 可靠性
- SLA：核心服务可用性 ≥ 99.9%
- 数据备份：每日全量 + 每小时增量，异地容灾

#### 4.4.2. 数据同步模式
- **单向同步**：HR系统为“主数据源”，支持：
  - 定时批量同步（每日）
  - 事件驱动（Webhook，实时性 < 5 秒）
- **双向同步**：仅限特定字段，冲突解决策略：时间戳优先

#### 4.4.3. 身份供给（SCIM）
- 完整支持 **SCIM 2.0**
- 自动同步用户生命周期事件至钉钉、飞书、Slack、Okta 等

#### 4.4.4. 业务流程联动（Webhook）
- 支持租户自定义事件（如“员工入职”、“请假批准”）
- 安全：HMAC-SHA256 签名验证
- 可靠性：指数退避重试（最多5次）+ 失败告警
- 管理界面：可视化配置、日志查看、重放

#### 4.4.5. API 安全管理
- 授权：OAuth 2.0 + JWT（Access Token）
- 网关：统一API网关处理认证、限流、日志、监控
- 权限：Scope 精细化控制（如 `payroll:read`, `employee:write`），遵循最小权限原则

---